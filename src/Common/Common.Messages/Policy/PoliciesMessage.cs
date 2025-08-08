namespace Common.Messages.Policy;

public class PoliciesMessage : IMessage
{
  public IEnumerable<PolicyMessage> Policies { get; set; } = [];
}
