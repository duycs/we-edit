using Application.Models;
using Infrastructure.Commands;
using Infrastructure.Models;

namespace Application.Commands
{
    public class AssignActionCommand : CommandResponse<InvokeResult>
    {
        public string MatchingAssignSetting { get; set; }
        public string RawSqlFilterJobStep { get; set; }
        public string ValidJobStepIsExpriedMethod { get; set; }
        public string RawSqlFilterStaff { get; set; }

        public AssignActionCommand(string matchingAssignSetting, string rawSqlFilterJobStep, string validJobStepIsExpriedMethod, string rawSqlFilterStaff)
        {
            MatchingAssignSetting = matchingAssignSetting;
            RawSqlFilterJobStep = rawSqlFilterJobStep;
            ValidJobStepIsExpriedMethod = validJobStepIsExpriedMethod;
            RawSqlFilterStaff = rawSqlFilterStaff;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
