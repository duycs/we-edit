using Domain;

public class UpdateStepStatusVM {
    public int StaffId {get;set;}
    public int JobId {get;set;}
    public int StepId {get;set;}

    public StepStatus Status {get;set;}
}