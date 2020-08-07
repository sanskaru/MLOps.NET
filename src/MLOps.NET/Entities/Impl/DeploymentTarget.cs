﻿using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Entity describing a deployment target
    /// </summary>
    public sealed class DeploymentTarget
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name"></param>
        public DeploymentTarget(string name)
        {
            DeploymentTargetId = Guid.NewGuid();
            Name = name;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid DeploymentTargetId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}