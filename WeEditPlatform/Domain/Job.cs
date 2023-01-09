using Newtonsoft.Json;

namespace Domain
{
    public class Job : EntityBase
    {
        public DateTime Date { get; set; }
        public Location Location { get; set; }

        public int CSOId { get; set; }
        public Staff CSO { get; set; }

        public string? JobId { get; set; }
        public string? Code { get; set; }
        public string? Instruction { get; set; }

        /// <summary>
        /// Data json format for input, output information
        /// </summary>
        public string? InputInfo { get; set; }
        public string? OutputInfo { get; set; }

        /// <summary>
        /// Number of input, output
        /// Eg: image pics
        /// </summary>
        public int InputNumber { get; set; }
        public int OutputNumber { get; set; }


        public DeliverType? DeliverType { get; set; }
        public DateTime Deadline { get; set; }

        public App? App { get; set; }

        public int ProductLevelId { get; set; }
        public ProductLevel ProductLevel { get; set; }


        /// <summary>
        /// From start to end job
        /// </summary>
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public JobStatus? Status { get; set; }


        public ICollection<JobStep>? JobSteps { get; set; }

        /// <summary>
        /// Steps: Editor1 -> QA1 -> Editor2 -> QA2 ->...
        /// </summary>
        public ICollection<Step>? Steps { get; set; }


        /// <summary>
        /// Create job with Todo status
        /// </summary>
        /// <param name="date"></param>
        /// <param name="location"></param>
        /// <param name="cso"></param>
        /// <param name="jobName"></param>
        /// <param name="picNumber"></param>
        /// <param name="instruction"></param>
        /// <param name="inputInfo"></param>
        /// <param name="inputNumber"></param>
        /// <param name="deliverType"></param>
        /// <param name="productLevel"></param>
        /// <param name="deadline"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public static Job Create(DateTime date, Location location, Staff cso,
            string jobName, string code, string instruction, string inputInfo, int inputNumber,
            DeliverType deliverType, ProductLevel productLevel, DateTime deadline, App app)
        {
            return new Job()
            {
                Date = date,
                Location = location,
                CSO = cso,
                JobId = CreateJobId(jobName, inputNumber),
                Code = code,
                Instruction = instruction,
                InputInfo = inputInfo,
                InputNumber = inputNumber,
                DeliverType = deliverType,
                ProductLevel = productLevel,
                Deadline = deadline,
                App = app,

                Status = JobStatus.Todo
            };
        }

        public Job UpdateStatus(JobStatus status)
        {
            Status = status;
            return this;
        }

        /// <summary>
        /// Set output and status
        /// </summary>
        /// <param name="outputNumber"></param>
        /// <param name="outputInfo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Job UpdateOutputAndStatus(int outputNumber, string outputInfo, JobStatus status)
        {
            OutputNumber = outputNumber;
            OutputInfo = outputInfo;
            Status = status;

            return this;
        }

        public bool IsExpried()
        {
            return EndTime > Deadline;
        }

        public int DoneInSeconds()
        {
            var doingTime = EndTime - StartTime;
            return doingTime.Seconds;
        }

        public Job Start()
        {
            StartTime = new DateTime();
            Status = JobStatus.Doing;
            return this;
        }

        /// <summary>
        /// End with done or pending status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public Job EndWithStatus(JobStatus status)
        {
            EndTime = new DateTime();
            Status = status;

            return this;
        }

        /// <summary>
        /// Only add a step, can not Steps.AddRange(steps)
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public Job AddStep(Step step)
        {
            if (Steps == null)
            {
                Steps = new List<Step>();
            }

            Steps.Add(step);

            return this;
        }

        public Job RemoveSteps(Step[] steps)
        {
            if (Steps != null && Steps.Any())
            {
                var removeStepCodes = steps.Select(x => x.Code);
                var result = Steps.Where(s => !removeStepCodes.Contains(s.Code)).ToList() ?? new List<Step>();
                Steps = result;
            }

            return this;
        }

        public Job AssignWorkerToStep(Staff worker, Step step, int? estimationInSeconds = 0)
        {
            // validate job has step need to assign
            if (JobSteps != null && JobSteps.Any())
            {
                var stepOfJob = JobSteps.FirstOrDefault(s => s.StepId == step.Id);
                if (stepOfJob != null)
                {
                    // worker at current shift
                    stepOfJob.SetWorkerAtShift(worker.Id, worker.GetCurrentShift().Id);

                    if (estimationInSeconds > 0)
                    {
                        stepOfJob.SetEstimationInSeconds(estimationInSeconds ?? 0);
                    }
                    else
                    {
                        stepOfJob.SetEstimationInSeconds(step.EstimationInSeconds);
                    }
                }
            }

            return this;
        }

        public Job OrderSteps()
        {
            if (Steps != null && Steps.Any())
            {
                Steps = Steps.OrderBy(x => x.OrderNumber).ToList();
            }

            return this;
        }

        /// <summary>
        /// Ex: LFN_Stockholmsvagen_29A_27pics_GB
        /// </summary>
        /// <returns></returns>
        public static string CreateJobId(string name, int picNumber)
        {
            return string.Format("{0}_{1}", name, picNumber);
        }

        /// <summary>
        /// To Job json string with all information
        /// </summary>
        /// <returns></returns>
        public string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}