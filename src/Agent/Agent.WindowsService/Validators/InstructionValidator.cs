using Agent.WindowsService.Domain;
using FluentValidation;

namespace Agent.WindowsService.Validators;

public class InstructionValidator : AbstractValidator<Instruction>
{
  public InstructionValidator()
  {
    RuleFor(instruction => instruction.Payload)
      .NotNull()
      .WithMessage("Payload cannot be null for ShellCommand instructions.")
      .When(instruction => instruction.Type == InstructionType.ShellCommand);

    RuleFor(instruction => instruction.Payload)
      .Must(payload => payload.ContainsKey("command") && !string.IsNullOrWhiteSpace(payload["command"]))
      .WithMessage("The 'command' field must be a non-empty string.")
      .When(instruction => instruction.Type == InstructionType.ShellCommand);
  }
}
