using Server.Application.Abstractions;
using Server.Application.Dtos;
using Server.Application.Dtos.Policy;

namespace Server.Application.Services;


public interface IPolicyService
{
	/// <summary>
	/// Get a specific policy by its ID.
	/// </summary>
	/// <param name="policyId"></param>
	/// <returns></returns>
	Task<PolicyDto?> GetPolicyAsync(long policyId);
	
	/// <summary>
	/// Gets all policies.
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<PolicyDto>> GetPoliciesAsync();
	
	/// <summary>
	/// Creates a new policy.
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	Task<PolicyDto?> CreatePolicyAsync(PolicyCreateRequest request);
	
	/// <summary>
	/// Updates an existing policy.
	/// </summary>
	/// <param name="policyId"></param>
	/// <param name="request"></param>
	/// <returns></returns>
	Task<PolicyDto?> UpdatePolicyAsync(long policyId, PolicyModifyRequest request);
}

public class PolicyService (IUnitOfWork unitOfWork) : IPolicyService
{
	public async Task<PolicyDto?> GetPolicyAsync(long policyId)
	{
		var policy = await unitOfWork.Policies.GetAsync(policyId);
		return policy?.ToDto();
	}

	public async Task<IEnumerable<PolicyDto>> GetPoliciesAsync()
	{
		var policies = await unitOfWork.Policies.GetAllAsync();
		return policies.Select(x => x.ToDto());
	}

	public async Task<PolicyDto?> CreatePolicyAsync(PolicyCreateRequest request)
	{
		var policyDomain = request.ToDomain();
		await unitOfWork.Policies.CreateAsync(policyDomain);
		await unitOfWork.SaveChangesAsync();
		
		var createdPolicyDto = await GetPolicyAsync(policyDomain.Id);
		return createdPolicyDto;
	}

	public async Task<PolicyDto?> UpdatePolicyAsync(long policyId, PolicyModifyRequest request)
	{
		var existingPolicyDomain = await unitOfWork.Policies.GetAsync(policyId);
		if (existingPolicyDomain is null)
			return null;
		
		var policyDomain = request.ToDomain(policyId);
		existingPolicyDomain.ModifyFrom(policyDomain);
		
		await unitOfWork.Policies.ModifyAsync(existingPolicyDomain);
		await unitOfWork.SaveChangesAsync();
		
		var updatedPolicyDto = await GetPolicyAsync(policyId);
		return updatedPolicyDto;
	}
}