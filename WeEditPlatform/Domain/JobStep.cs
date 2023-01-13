using System.Text.Json.Serialization;

namespace Domain
{
    public class JobStep : EntityBase
    {
        public int JobId { get; set; }
        public Job Job { get; set; }

        public int StepId { get; set; }
        public Step Step { get; set; }


        /// <summary>
        /// Data json format for input, output information
        /// </summary>
        public string? InputInfo { get; set; }
        public int InputNumber { get; set; }

        public string? OutputInfo { get; set; }
        public int OutputNumber { get; set; }


        // worker do at shift
        public int? WorkerId { get; set; }
        public Staff? Worker { get; set; }

        public int? ShiftId { get; set; }
        public Shift? Shift { get; set; }


        // progress and status
        public int EstimationInSeconds { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public StepStatus? Status { get; set; }


        public static JobStep Create(int JobId, int stepId)
        {
            return new JobStep()
            {
                JobId = JobId,
                StepId = stepId,
            };
        }

        public void UpdateOutputAndStatus(int outputNumber, string outputInfo, StepStatus status)
        {
            OutputNumber = outputNumber;
            OutputInfo = outputInfo;
            Status = status;
        }

        public void UpdateStatus(StepStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// Start doing
        /// </summary>
        public void StartDoing()
        {
            StartTime = DateTime.UtcNow;
            Status = StepStatus.Doing;
        }

        /// <summary>
        /// End with approved or rejected status
        /// </summary>
        /// <param name="status"></param>
        public void EndWithStatus(StepStatus status)
        {
            EndTime = DateTime.UtcNow;
            Status = status;
        }

        public void ResetTimeWithStatus(StepStatus status)
        {
            StartTime = default(DateTime);
            EndTime = default(DateTime);
            Status = status;
        }

        public JobStep SetEstimationInSeconds(int estimationInSeconds)
        {
            EstimationInSeconds = estimationInSeconds;
            return this;
        }

        /// <summary>
        /// Expried when startTime + estimation > current
        /// </summary>
        /// <returns></returns>
        public bool IsExpried()
        {
            if (StartTime == default(DateTime) || StartTime == null) return true;

            return StartTime.AddSeconds(EstimationInSeconds) > DateTime.UtcNow;
        }

        public void SetWorkerAtShift(int workerId, int shiftId)
        {
            WorkerId = workerId;
            ShiftId = shiftId;
        }

        public void SetNoneWorkerShift()
        {
            WorkerId = 0;
            Worker = null;
            ShiftId = 0;
            Shift = null;
        }

    }
}
