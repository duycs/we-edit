using Infrastructure.Commands;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Operations.Actions.GetJobStepAction
{
    public class GetJobStepActionCommand : CommandResponse<InvokeResult>
    {
        public string RawSqlFilterJobStep { get; set; }

        public GetJobStepActionCommand(string rawSqlFilterJobStep)
        {
            RawSqlFilterJobStep = rawSqlFilterJobStep;
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(RawSqlFilterJobStep);
        }
    }
}
