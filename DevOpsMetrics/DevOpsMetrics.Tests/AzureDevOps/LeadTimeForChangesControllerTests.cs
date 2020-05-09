﻿using DevOpsMetrics.Service.Controllers;
using DevOpsMetrics.Service.Models;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevOpsMetrics.Tests.AzureDevOps
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("IntegrationTest")]
    [TestClass]
    public class LeadTimeForChangesControllerTests
    {
        private TestServer _server;
        public HttpClient Client;
        public IConfigurationRoot Configuration;

        [TestInitialize]
        public void TestStartUp()
        {
            IConfigurationBuilder config = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json");
            Configuration = config.Build();

            //Setup the test server
            _server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseConfiguration(Configuration)
                .UseStartup<DevOpsMetrics.Service.Startup>());
            Client = _server.CreateClient();
            Client.BaseAddress = new Uri(Configuration["AppSettings:WebServiceURL"]);
        }

        //[TestCategory("ControllerTest")]
        //[TestMethod]
        //public async Task AzLeadTimeControllerIntegrationTest()
        //{
        //    //Arrange
        //    bool getSampleData = true;
        //    string patToken = Configuration["AppSettings:AzureDevOpsPatToken"];
        //    string organization = "samsmithnz";
        //    string project = "SamLearnsAzure";
        //    string repositoryId = "SamLearnsAzure";
        //    string branch = "refs/heads/master";
        //    string buildId = "3673"; //SamLearnsAzure.CI
        //    LeadTimeForChangesController controller = new LeadTimeForChangesController();

        //    //Act
        //    List<LeadTimeForChangesModel> list = await controller.GetAzureDevOpsLeadTimeForChanges(getSampleData, patToken, organization, project, repositoryId, branch, buildId);

        //    //Assert
        //    Assert.IsTrue(list != null);
        //    Assert.IsTrue(list.Count > 0);
        //    Assert.IsTrue(list[0].Branch != null);
        //}

        [TestCategory("APITest")]
        [TestMethod]
        public async Task AzLeadTimeControllerAPIIntegrationTest()
        {
            //Arrange
            bool getSampleData = false;
            string patToken = Configuration["AppSettings:PatToken"];
            string organization = "samsmithnz";
            string project = "SamLearnsAzure";
            string repositoryId = "SamLearnsAzure";
            string branch = "refs/heads/master";
            string buildId = "3673"; //SamLearnsAzure.CI

            //Act
            string url = $"/api/LeadTimeForChanges/GetAzureDevOpsLeadTimeForChanges?getSampleData={getSampleData}&patToken={patToken}&organization={organization}&project={project}&repositoryId={repositoryId}& branch={branch}&buildId={buildId}";
            TestResponse<List<LeadTimeForChangesModel>> httpResponse = new TestResponse<List<LeadTimeForChangesModel>>();
            List<LeadTimeForChangesModel> list = await httpResponse.GetResponse(Client, url);

            //Assert
            Assert.IsTrue(list != null);
            Assert.IsTrue(list.Count > 0);
            //Assert.IsTrue(list[0].branch != null);
        }

    }
}