﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;
using FinePrint;
using FinePrint.Utilities;

namespace ContractConfigurator
{
    /// <summary>
    /// ContractRequirement to provide requirement for a player unlocking a "type" of module.
    /// </summary>
    public class PartModuleTypeUnlockedRequirement : ContractRequirement
    {
        protected List<string> partModuleType;

        public override bool Load(ConfigNode configNode)
        {
            // Load base class
            bool valid = base.Load(configNode);

            // Check on active contracts too
            checkOnActiveContract = configNode.HasValue("checkOnActiveContract") ? checkOnActiveContract : true;

            valid &= ConfigNodeUtil.ParseValue<List<string>>(configNode, "partModuleType", ref partModuleType, this);

            return valid;
        }

        public override bool RequirementMet(ConfiguredContract contract)
        {
            // Late validation
            partModuleType.All(Validation.ValidatePartModuleType);

            // Actual check
            return partModuleType.All(ProgressUtilities.HaveModuleTypeTech);
        }
    }
}
