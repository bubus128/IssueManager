using System.Runtime.Serialization;

namespace IssueManager.Core.Enums;
public enum SourceType
{
	[EnumMember(Value = "GitHub")]
	GitHub,

	[EnumMember(Value = "GitLab")]
	GitLab
}
