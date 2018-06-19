﻿using System;
using System.Drawing;
using System.IO;
using Ghpr.Core;
using Ghpr.Core.Common;
using Ghpr.Core.Extensions;
using Ghpr.Core.Interfaces;
using Ghpr.LocalFileSystem.Entities;
using Ghpr.LocalFileSystem.Extensions;
using Ghpr.LocalFileSystem.Interfaces;
using Ghpr.LocalFileSystem.Mappers;
using Ghpr.LocalFileSystem.Providers;
using Newtonsoft.Json;

namespace Ghpr.LocalFileSystem.Services
{
    public class FileSystemDataService : IDataService
    {
        private ILocationsProvider _locationsProvider;
        private ILogger _logger;

        public void Initialize(ReporterSettings settings, ILogger logger)
        {
            _locationsProvider = new LocationsProvider(settings.OutputPath);
            _logger = logger;
        }

        public void SaveRun(RunDto runDto)
        {
            var run = runDto.Map();
            var runFullPath = run.Save(_locationsProvider.RunsPath);
            _logger.Info($"Run was saved: '{runFullPath}'");
            var runsInfoFullPath = run.RunInfo.SaveRunInfo(_locationsProvider);
            _logger.Info($"Runs Info was saved: '{runsInfoFullPath}'");
            _logger.Debug($"Run data was saved correctly: {JsonConvert.SerializeObject(run, Formatting.Indented)}");
        }

        public void SaveScreenshot(TestScreenshotDto screenshotDto)
        {
            var testScreenshot = screenshotDto.Map();
            using (var ms = new MemoryStream(Convert.FromBase64String(testScreenshot.Base64Data)))
            using (var image = Image.FromStream(ms))
            {
                var screenPath = _locationsProvider.GetScreenshotPath(testScreenshot.TestGuid.ToString());
                screenPath.Create();
                var screenName = LocationsProvider.GetScreenshotFileName(testScreenshot.TestScreenshotInfo.Date, testScreenshot.Format);
                var file = Path.Combine(screenPath, screenName);
                var screen = new Bitmap(image);
                screen.Save(file);
                var fileInfo = new FileInfo(file);
                fileInfo.Refresh();
                fileInfo.CreationTime = testScreenshot.TestScreenshotInfo.Date;
                _logger.Info($"Screenshot was saved: '{file}'");
            }
        }

        public void SaveReportSettings(ReportSettingsDto reportSettingsDto)
        {
            var reportSettings = reportSettingsDto.Map();
            var fullPath = reportSettings.Save(_locationsProvider);
            _logger.Info($"Report settings were saved: '{fullPath}'");
        }

        public void SaveTestRun(TestRunDto testRunDto, TestOutputDto testOutputDto)
        {
            var testRun = testRunDto.Map();
            var imgFolder = _locationsProvider.GetScreenshotPath(testRun.TestInfo.Guid.ToString());
            if (Directory.Exists(imgFolder))
            {
                var imgFiles = new DirectoryInfo(imgFolder).GetFiles("*.*");
                foreach (var imgFile in imgFiles)
                {
                    if (imgFile.CreationTime > testRun.TestInfo.Start)
                    {
                        testRun.Screenshots.Add(new TestScreenshot
                        {
                            TestScreenshotInfo = new SimpleItemInfo()
                            {
                                Date = imgFile.CreationTime,
                                ItemName = LocationsProvider.GetScreenshotFileName(imgFile.CreationTime, imgFile.Extension)
                            },
                            Format = imgFile.Extension.Replace(".", "").ToLower()
                        });
                    }
                }
            }
            var testRunFullPath = testRun.Save(_locationsProvider.GetTestPath(testRun.TestInfo.Guid.ToString()));
            _logger.Info($"Test run was saved: '{testRunFullPath}'");
            var testRunsInfoFullPath = testRun.TestInfo.SaveTestInfo(_locationsProvider);
            _logger.Info($"Test runs Info was saved: '{testRunsInfoFullPath}'");
            _logger.Debug($"Test run data was saved correctly: {JsonConvert.SerializeObject(testRun, Formatting.Indented)}");
        }
    }
}