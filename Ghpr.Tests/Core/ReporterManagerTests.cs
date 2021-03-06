﻿using System;
using Ghpr.Core.Core.Common;
using Ghpr.Core.Core.Enums;
using Ghpr.Core.Core.Settings;
using NUnit.Framework;

namespace Ghpr.Tests.Core
{
    [TestFixture]
    public class ReporterManagerTests
    {
        [Test]
        public void CreationTest()
        {
            ReporterManager.TearDown();
            ReporterManager.Initialize(new MockTestDataProvider());
            Assert.AreEqual("C:\\_GHPReporter_Core_Report", ReporterManager.OutputPath);
        }

        [TestCase(TestingFramework.NUnit, "C:\\_GHPReporter_NUnit_Report")]
        [TestCase(TestingFramework.MSTest, "C:\\_GHPReporter_MSTest_Report")]
        [TestCase(TestingFramework.MSTestV2, "C:\\_GHPReporter_MSTestV2_Report")]
        [TestCase(TestingFramework.SpecFlow, "C:\\_GHPReporter_SpecFlow_Report")]
        public void CanCreateByFramework(TestingFramework framework, string outputPath)
        {
            ReporterManager.TearDown();
            ReporterManager.Initialize(framework, new MockTestDataProvider());
            Assert.AreEqual(outputPath, ReporterManager.OutputPath);
        }

        [Test]
        public void CanCreateWithSettings()
        {
            ReporterManager.TearDown();
            var s = new ReporterSettings
            {
                DefaultSettings = new ProjectSettings
                {
                    RunGuid = Guid.NewGuid().ToString(),
                    DataServiceFile = "Ghpr.LocalFileSystem.dll",
                    LoggerFile = "",
                    OutputPath = @"\\server\folder",
                    ProjectName = "cool project",
                    RealTimeGeneration = true,
                    ReportName = "report name",
                    Retention = new RetentionSettings
                    {
                        Amount = 3,
                        Till = DateTime.Now
                    },
                    RunName = "run name",
                    RunsToDisplay = 7
                }
            };
            ReporterManager.Initialize(s, new MockTestDataProvider());
            Assert.AreEqual(s.DefaultSettings.OutputPath, ReporterManager.OutputPath);
        }

        [Test]
        public void CanSetTestDataProvider()
        {
            ReporterManager.TearDown();
            ReporterManager.Initialize(new MockTestDataProvider());
            Assert.DoesNotThrow(() => ReporterManager.SetTestDataProvider(new MockTestDataProviderWithException()));
        }

        [Test]
        public void Process()
        {
            ReporterManager.TearDown();
            ReporterManager.Initialize(new MockTestDataProvider());
            ReporterManager.RunStarted();
            ReporterManager.TestStarted(new TestRunDto());
            ReporterManager.TestFinished(new TestRunDto(), new TestOutputDto
            {
                Output = "test output",
                SuiteOutput = "suite output",
                TestOutputInfo = new SimpleItemInfoDto
                    {
                        Date = DateTime.Now,
                        ItemName = "item name"
                    }
            });
            ReporterManager.RunFinished();
        }
    }
}