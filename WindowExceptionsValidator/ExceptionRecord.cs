namespace WindowExceptionsValidation;

public record ExceptionRecord
{
    public string windowExceptionId { get; set; }
    public string originalWindowId { get; set; }
    public string originalDeliveryDay { get; set; }
    public string replacementDeliveryDay { get; set; }
    public string originalDeliveryDate { get; set; }
    public string replacementWindowId { get; set; }
    public string replacementDeliveryDate { get; set; }
    public string reason { get; set; }
    public string canDonate { get; set; }
    public string donationPartnerIds { get; set; }
}