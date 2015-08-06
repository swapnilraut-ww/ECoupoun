//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECoupoun.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Provider
    {
        public Provider()
        {
            this.APIDetails = new HashSet<APIDetail>();
            this.ProductLinks = new HashSet<ProductLink>();
            this.ProductLinks1 = new HashSet<ProductLink>();
            this.ProductPricings = new HashSet<ProductPricing>();
            this.ProductViewDetails = new HashSet<ProductViewDetail>();
            this.ProductViewDetails1 = new HashSet<ProductViewDetail>();
        }
    
        public int ProviderId { get; set; }
        public string Name { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public bool IsActive { get; set; }
    
        public virtual ICollection<APIDetail> APIDetails { get; set; }
        public virtual ProviderPriority ProviderPriority { get; set; }
        public virtual ICollection<ProductLink> ProductLinks { get; set; }
        public virtual ICollection<ProductLink> ProductLinks1 { get; set; }
        public virtual ICollection<ProductPricing> ProductPricings { get; set; }
        public virtual ICollection<ProductViewDetail> ProductViewDetails { get; set; }
        public virtual ICollection<ProductViewDetail> ProductViewDetails1 { get; set; }
    }
}
