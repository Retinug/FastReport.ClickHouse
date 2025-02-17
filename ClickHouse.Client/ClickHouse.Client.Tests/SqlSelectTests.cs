﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ClickHouse.Client.ADO.Readers;
using ClickHouse.Client.Types;
using ClickHouse.Client.Utility;
using NUnit.Framework;

namespace ClickHouse.Client.Tests
{
    [Parallelizable]
    [TestFixture(ClickHouseConnectionDriver.Binary, true)]
    [TestFixture(ClickHouseConnectionDriver.JSON, true)]
    [TestFixture(ClickHouseConnectionDriver.TSV, true)]
    [TestFixture(ClickHouseConnectionDriver.Binary, false)]
    [TestFixture(ClickHouseConnectionDriver.JSON, false)]
    [TestFixture(ClickHouseConnectionDriver.TSV, false)]
    public class SqlSelectTests
    {
        private readonly ClickHouseConnectionDriver driver;
        private readonly DbConnection connection;

        public SqlSelectTests(ClickHouseConnectionDriver driver, bool useCompression)
        {
            this.driver = driver;
            connection = TestUtilities.GetTestClickHouseConnection(driver, useCompression);
        }

        public static IEnumerable<TestCaseData> SimpleSelectQueries => TestUtilities.GetDataTypeSamples()
            .Select(sample => new TestCaseData($"SELECT {sample.ExampleExpression}") { ExpectedResult = sample.ExampleValue });

        [Test]
        [TestCaseSource(typeof(SqlSelectTests), nameof(SimpleSelectQueries))]
        public async Task<object> ShouldExecuteSimpleSelectQuery(string sql)
        {
            using var reader = await connection.ExecuteReaderAsync(sql);
            reader.AssertHasFieldCount(1);
            var result = reader.GetEnsureSingleRow().Single();

            return result;
        }

        [Test]
        public async Task ShouldSelectMultipleColumns()
        {
            using var reader = await connection.ExecuteReaderAsync("SELECT 1 as a, 2 as b, 3 as c");

            reader.AssertHasFieldCount(3);
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, reader.GetFieldNames());
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, reader.GetEnsureSingleRow());
        }

        [Test]
        [TestCase("Добрый день")]
        [TestCase("¿Qué tal?")]
        [TestCase("你好")]
        [TestCase("こんにちは")]
        [TestCase("⌬⏣")]
        public async Task ShouldSelectUnicode(string input)
        {
            using var reader = await connection.ExecuteReaderAsync($"SELECT '{input}'");

            reader.AssertHasFieldCount(1);
            var result = reader.GetEnsureSingleRow().Single();
            Assert.AreEqual(input, result);
        }

        [Test]
        public async Task ShouldSelectEmptyDataset()
        {
            using var reader = await connection.ExecuteReaderAsync("SELECT 1 LIMIT 0");

            reader.AssertHasFieldCount(1);
            //Assert.IsFalse(reader.HasRows);
            Assert.IsFalse(reader.Read());
        }

        [Test]
        public async Task DateTimeSelectShouldHaveCorrectTimezone()
        {
            using var reader = await connection.ExecuteReaderAsync("SELECT toDateTime(1577836800, 'Asia/Sakhalin')");

            reader.AssertHasFieldCount(1);
            var datetime = (DateTime)reader.GetEnsureSingleRow().Single();
            if (datetime.Kind == DateTimeKind.Utc)
            {
                Assert.AreEqual(new DateTime(2020, 01, 01, 0, 0, 0, DateTimeKind.Utc), datetime.ToUniversalTime());
                Assert.AreEqual(ClickHouseConnectionDriver.Binary, driver);
            }
            else
            {
                Assert.AreEqual(new DateTime(2020, 01, 01, 11, 0, 0, DateTimeKind.Unspecified), datetime);
                Assert.AreEqual(DateTimeKind.Unspecified, datetime.Kind);
            }
        }

        [Test]
        public async Task DateTime64SelectShouldHaveCorrectTimezone()
        {
            using var reader = await connection.ExecuteReaderAsync("SELECT toDateTime64(1577836800, 3, 'Asia/Sakhalin')");

            reader.AssertHasFieldCount(1);
            var datetime = (DateTime)reader.GetEnsureSingleRow().Single();
            if (datetime.Kind == DateTimeKind.Utc)
            {
                Assert.AreEqual(new DateTime(2020, 01, 01, 0, 0, 0, DateTimeKind.Utc), datetime.ToUniversalTime());
            }
            else
            {
                Assert.AreEqual(new DateTime(2020, 01, 01, 11, 0, 0, DateTimeKind.Unspecified), datetime);
                Assert.AreEqual(DateTimeKind.Unspecified, datetime.Kind);
            }
        }

        [Test]
        public async Task DateTimeOffsetShouldProduceCorrectOffset()
        {
            using var reader = (ClickHouseDataReader)await connection.ExecuteReaderAsync("SELECT toDateTime(1577836800, 'Asia/Sakhalin')");
            reader.AssertHasFieldCount(1);
            Assert.IsTrue(reader.Read());
            var dto = reader.GetDateTimeOffset(0);
            Assert.AreEqual(TimeSpan.FromHours(11), dto.Offset);
            Assert.AreEqual(new DateTime(2020, 01, 01, 0, 0, 0, DateTimeKind.Utc), dto.UtcDateTime);
        }

        [Test]
        public async Task ShouldSelectNumericTypes()
        {
            var types = Enum.GetValues(typeof(ClickHouseTypeCode))
                .Cast<ClickHouseTypeCode>()
                .Select(dt => dt.ToString())
                .Where(dt => dt.Contains("Int") || dt.Contains("Float"))
                .Select(dt => $"to{dt.ToString()}(55)")
                .ToArray();
            var sql = $"select {string.Join(',', types)}";

            using var reader = await connection.ExecuteReaderAsync(sql);
            Assert.AreEqual(types.Length, reader.FieldCount);

            var data = reader.GetEnsureSingleRow();
            Assert.AreEqual(Enumerable.Repeat(55.0d, data.Length), data);
        }

        [Test]
        public async Task ShouldSelectSingleColumnRange()
        {
            const int count = 100;
            using var reader = await connection.ExecuteReaderAsync($"SELECT number FROM system.numbers LIMIT {count}");

            var results = new List<int>();

            Assert.IsTrue(reader.HasRows);
            reader.AssertHasFieldCount(1);
            Assert.AreEqual(typeof(ulong), reader.GetFieldType(0));

            while (reader.Read())
                results.Add(reader.GetInt32(0)); // Intentional conversion to int32

            CollectionAssert.AreEqual(Enumerable.Range(0, count), results);
        }

        [Test]
        [NonParallelizable]
        public async Task ShouldSelectNestedDataType()
        {
            await connection.ExecuteStatementAsync("CREATE DATABASE IF NOT EXISTS test");
            await connection.ExecuteStatementAsync("TRUNCATE TABLE IF EXISTS test.nested");
            await connection.ExecuteStatementAsync("CREATE TABLE IF NOT EXISTS test.nested(nested_v Nested (int16_v Int16, uint32_v UInt32, dtime_v DateTime, string_v String)) ENGINE = Memory");

            using var reader = await connection.ExecuteReaderAsync("SELECT nested_v.int16_v, nested_v.uint32_v, nested_v.dtime_v, nested_v.string_v FROM test.nested");
        }

        [Test]
        public async Task ShouldCancelRunningAsyncQuery()
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT sleep(3)";
            var task = command.ExecuteScalarAsync();
            await Task.Delay(50).ConfigureAwait(false);
            command.Cancel();

            try
            {
                await task;
                Assert.Fail("Expected to receive TaskCancelledException from task");
            }
            catch (TaskCanceledException)
            {
                // Correct
            }
        }
        
        [Test]
        public async Task ShouldGetReaderColumnSchema()
        {
            using var reader = await connection.ExecuteReaderAsync("SELECT 1 as num, 'a' as str");
            var schema = reader.GetColumnSchema();
            Assert.AreEqual(2, schema.Count);
            Assert.AreEqual("num", schema[0].ColumnName);
            Assert.AreEqual("str", schema[1].ColumnName);
        }

        [Test]
        public async Task ShouldGetReaderSchemaTable()
        {
            using var reader = await connection.ExecuteReaderAsync("SELECT 1 as num, 'a' as str");
            var schema = reader.GetSchemaTable();
            Assert.AreEqual(2, schema.Rows.Count);
        }
    }
}
