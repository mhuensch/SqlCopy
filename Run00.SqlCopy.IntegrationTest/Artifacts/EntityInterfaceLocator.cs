using Run00.SqlCopy;
using Run00.SqlCopy.IntegrationTest.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy.IntegrationTest
{
	public class EntityInterfaceLocator : IEntityInterfaceLocator
	{
		IEnumerable<Type> IEntityInterfaceLocator.GetInterfacesForEntity(string entityName)
		{
			if (string.Equals(entityName, "dbo.Samples", StringComparison.InvariantCultureIgnoreCase))
				return new[] { typeof(IOwnedEntity) };

			if (string.Equals(entityName, "dbo.SampleChilds", StringComparison.InvariantCultureIgnoreCase))
				return new[] { typeof(IOwnedEntity), typeof(IChildOwners) };

			if (string.Equals(entityName, "dbo.SampleGrandChilds", StringComparison.InvariantCultureIgnoreCase))
				return new[] { typeof(IChildOwnedEntity) };

			if (string.Equals(entityName, "dbo.tenant", StringComparison.InvariantCultureIgnoreCase))
				return new[] { typeof(IIdEntity) };

			//if(string.Equals(entityName, "dbo.user", StringComparison.InvariantCultureIgnoreCase))
			if(_tenantTables.Any(x => x.Equals(entityName.Replace("dbo.", string.Empty))))
				return new[] { typeof(ITenantEntity) };

			//if (_platformTenantTables.Any(x => x.Equals(entityName)))
			if(_platformGuidNonNull.Any(x => x.Equals(entityName)))
				return new[] { typeof(IGuidTenantIdEntity) };

			//if (string.Equals(entityName, "dbo.user_group_user"))
			//	return new[] { typeof(ILinkEntity) };

			return new[] { typeof(IIgnoreTable) };
		}

		private string[] _platformTenantTables = new[] { "Accelerator.Alias",
"Accelerator.Opportunity",
"Accelerator.OpportunityBuyerAlignment",
"Accelerator.OpportunityRecommendation",
"Accelerator.OpportunityStage",
"Accelerator.OpportunityStaticResourceFile",
"Accelerator.OpportunityStaticResourceUser",
"Accelerator.OpportunityTask",
"Accelerator.OpportunityTaskFeedback",
"Accelerator.OpportunityTaskGroup",
"Accelerator.Template",
"Accelerator.TemplateBuyerAlignment",
"Accelerator.TemplateField",
"Accelerator.TemplateFieldValue",
"Accelerator.TemplateRecommendation",
"Accelerator.TemplateStage",
"Accelerator.TemplateStaticResourceFile",
"Accelerator.TemplateStaticResourceFileUnprocessed",
"Accelerator.TemplateStaticResourceUser",
"Accelerator.TemplateTask",
"Accelerator.TemplateTaskGroup",
"Accelerator.TemplateVocabularyAlias",
"Accelerator.Vocabulary",
"Catalog.Catalog",
"Catalog.CatalogResource",
"Catalog.CatalogResourceArchive",
"Catalog.CatalogResourceHistory",
"Catalog.CatalogResourceSavoClassicAsset",
"Catalog.CatalogState",
"Catalog.Category",
"Catalog.CategoryHistory",
"Integrations.IntegrationAttribute",
"Integrations.IntegrationAttributeMapping",
"Integrations.IntegrationUser",
"Integrations.RegisteredIntegration",
"Integrations.SupportedIntegration",
"Platform.Css",
"Platform.EntityAction",
"Platform.Logo",
"Platform.UserProfile",
"Proposals.Flow",
"Proposals.FlowAttribute",
"Proposals.FlowForm",
"Proposals.FlowGroup",
"Proposals.FlowInstanceAttribute",
"Proposals.FlowInstanceDocument",
"Proposals.FlowInstanceVariableValue",
"Proposals.FlowTemplateDocument",
"Proposals.FormDocument",
"Proposals.ProposalDocument",
"RFP.AlertGroup",
"RFP.CustomAnswerRelatedCatalogResources",
"RFP.CustomField",
"RFP.CustomFieldInstance",
"RFP.IconImage",
"RFP.RFP",
"RFP.RFPDocument",
"RFP.RFPDocumentDelta",
"RFP.RFPHistory",
"RFP.RFPQuestion",
"RFP.RFPQuestionAnswer",
"RFP.RFPQuestionHistory",
"RFP.RFPSection",
"RFP.RFPSectionHistory",
"RFP.RFPStatusHistory",
"SocialIntelligence.ApplicationObjectChannelFeeds",
"SocialIntelligence.ApplicationObjectFeeds",
"SocialIntelligence.ChannelFeed",
"SocialIntelligence.ChannelFeedAttributeValue",
"SocialIntelligence.ChannelUser",
"SocialIntelligence.ChannelUserAttributeValue",
"SocialIntelligence.RegisteredChannel",
"Teams.Answer",
"Teams.Discussion",
"Teams.Event",
"Teams.Image",
"Teams.Initiative",
"Teams.InitiativeInvite",
"Teams.Link",
"Teams.Note",
"Teams.Question",
"Teams.Recommendation",
"Teams.Reply",
"Teams.Room",
"Teams.RoomActivities",
"Teams.RoomCustomStage",
"Teams.RoomDetailsAlias",
"Teams.RoomInvite",
"Teams.RoomTemplate",
"Teams.Task",
"Teams.TemplateCategory",
		};

		private string[] _platformGuidNonNull = new[] { 

"Insight.Dashboard", 
"Lifecycle.Action",
"Lifecycle.DefaultRule",
"Lifecycle.EntityLifecycle",
"Lifecycle.Instruction",
"Lifecycle.Lifecycle",
"Lifecycle.Note",
"Lifecycle.Stage",
"Lifecycle.Task",
"Platform.ApplicationSetting",
"Platform.ApplicationSettingDefault",
"Proposals.FlowInstance",
"Proposals.FlowInstanceAttributeValue",
"Proposals.FlowInstanceVariable",
"RFP.AlertEmailsRFPStatusRFP",
"RFP.AlertGroupRFPStatus",
"Platform.TenantApplication",
"RFP.RFPStatus",
"Security.RoleConfiguration",
"Security.RoleConfigurationDefault",
"Security.TenantPermission",
"Security.TenantRole",
"Teams.AssociatedAsset",
"Teams.DocumentContainer",
		};

		private string[] _tenantTables = new[] { "email_cue_to_strip",
"question",
"report",
"user_login_tenant_login_count",
"report_view",
"expertise",
"request",
"external_application_tenant",
"externalresource",
"user_profile_preference",
"feature",
"Analytics_Customer_Tenant",
"Analytics_Tenant_Scorecard",
"watermark",
"asset_kit",
"guide",
//"audit",
"request_job_subscription",
"html_migration",
"Indexer_Incremental_Files",
"indexer_incremental_requests",
"integration_endpoint",
"certificate",
"library",
"scheduled_rebuild_index",
//"search_history", -- causes sql timeouts i guess the table has a lot of rows
"comment",
"search_result_log",
"limelight_bitrate_tenant",
"smart_list",
"limelight_folder_tenant",
//"content_variable_setting",
"menu_item",
"contract_tenant",
"custom_field",
"mobile_menu_item",
//"subdomain",
"tag",
"tag_list",
"custom_page",
"template",
"oa_component_migration",
"OA_Migration",
"tenant_permission",
"tenant_preference",
"document",
"tenant_published_updates",
"topic",
"user",
"user_aspnetuser",
"post",
"user_group",
"post_type",
"promotion_trigger" };

	}
}