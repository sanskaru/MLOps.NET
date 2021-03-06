﻿using FluentAssertions;
using Microsoft.ML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    public class EndToEndTests : RepositoryTests
    {
        [TestMethod]
        public async Task DeployModel_GivenACompleteRun_ShouldDeployModel()
        {
            //Arrange and Act
            var mlContext = new MLContext(seed: 2);

            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var run = await sut.LifeCycle.CreateRunAsync(experimentId);

            var data = mlContext.Data.LoadFromEnumerable(GetSampleDataForTraining());
            await sut.Data.LogDataAsync(run.RunId, data);

            var trainer = mlContext.BinaryClassification.Trainers
                .LbfgsLogisticRegression(labelColumnName: "Label", featureColumnName: "Features");

            await sut.Training.LogHyperParametersAsync(run.RunId, trainer);

            var model = trainer.Fit(data);

            var predicitions = model.Transform(data);
            var metrics = mlContext.BinaryClassification.Evaluate(predicitions, labelColumnName: "Label");

            await sut.Evaluation.LogMetricsAsync(run.RunId, metrics);
            await sut.Evaluation.LogConfusionMatrixAsync(run.RunId, metrics.ConfusionMatrix);

            await sut.Model.UploadAsync(run.RunId, @"Data/model.txt");

            var runArtifact = sut.Model.GetRunArtifacts(run.RunId).First();

            var registeredModel = await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team", "Model Registered By Test");
            var deploymentTarget = await sut.Deployment.CreateDeploymentTargetAsync("Test");
            await sut.Deployment.DeployModelToUriAsync(deploymentTarget, registeredModel, "The MLOps Team");

            deploymentTarget = sut.Deployment.GetDeploymentTargets().First();
            var experiment = sut.LifeCycle.GetExperiment("test");
            var loggedData = sut.Data.GetData(run.RunId);

            //Assert
            deploymentTarget.Deployments.Should().NotBeNull();

            loggedData.Should().NotBeNull();
            loggedData.DataSchema.Should().NotBeNull();
            loggedData.DataSchema.DataColumns.Should().NotBeNull();

            experiment.Runs.Should().NotBeNull();
            experiment.Runs.First().RunArtifacts.Should().NotBeNull();
            experiment.Runs.First().Metrics.Should().NotBeNull();
            experiment.Runs.First().HyperParameters.Should().NotBeNull();
            experiment.Runs.First().ConfusionMatrix.Should().NotBeNull();
        }

        private static List<DataPoint> GetSampleDataForTraining()
        {
            return new List<DataPoint>()
            {
                new DataPoint { Features = new float[3] {0, 2, 1} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 3} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 4} , Label = true  },
                new DataPoint { Features = new float[3] {0, 2, 1} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 2} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 3} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 4} , Label = true  },
                new DataPoint { Features = new float[3] {1, 0, 0} , Label = true  }
            };
        }
    }
}
