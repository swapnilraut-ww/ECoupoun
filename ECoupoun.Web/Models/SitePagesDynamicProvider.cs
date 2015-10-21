using ECoupoun.Data;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECoupoun.Web.Models
{
    public class SitePagesDynamicProvider : DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            using (var ef = new ECoupounEntities())
            {
                // Create a node for each sites 
                foreach (var obj in ef.Categories)
                {
                    DynamicNode dynamicNode;

                    dynamicNode = new DynamicNode();
                    dynamicNode.Title = obj.MappingName;
                    dynamicNode.ParentKey = "CulturePages";
                    dynamicNode.RouteValues.Add("parentCategory", obj.MappingName);
                    dynamicNode.RouteValues.Add("categoryName", obj.MappingName);
                    yield return dynamicNode;
                }
            }
        }
    }
}