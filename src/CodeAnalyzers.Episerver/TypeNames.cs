namespace CodeAnalyzers.Episerver
{
    public static class TypeNames
    {
        public static readonly string IContentDataMetadataName = "EPiServer.Core.IContentData";
        
        public static readonly string CatalogContentBaseMetadataName = "EPiServer.Commerce.Catalog.ContentTypes.CatalogContentBase";
        public static readonly string NodeContentMetadataName = "EPiServer.Commerce.Catalog.ContentTypes.NodeContent";
        public static readonly string ProductContentMetadataName = "EPiServer.Commerce.Catalog.ContentTypes.ProductContent";
        public static readonly string PackageContentMetadataName = "EPiServer.Commerce.Catalog.ContentTypes.PackageContent";
        public static readonly string BundleContentMetadataName = "EPiServer.Commerce.Catalog.ContentTypes.BundleContent";

        public static readonly string PageDataMetadataName = "EPiServer.Core.PageData";
        public static readonly string BlockDataMetadataName = "EPiServer.Core.BlockData";
        public static readonly string ContentAreaTypeMetadataName = "EPiServer.Core.ContentArea";      

        public static readonly string PromotionDataMetadataName = "EPiServer.Commerce.Marketing.PromotionData";
        public static readonly string SalesCampaignMetadataName = "EPiServer.Commerce.Marketing.SalesCampaign";

        public static readonly string ContentTypeMetadataName = "EPiServer.DataAnnotations.ContentTypeAttribute";
        public static readonly string CatalogContentTypeMetadataName = "EPiServer.Commerce.Catalog.DataAnnotations.CatalogContentTypeAttribute";                            
        public static readonly string ImageUrlMetadataName = "EPiServer.DataAnnotations.ImageUrlAttribute";
        public static readonly string DisplayMetadataName = "System.ComponentModel.DataAnnotations.DisplayAttribute";
        public static readonly string AvailableContentTypesMetadataName = "EPiServer.DataAnnotations.AvailableContentTypesAttribute";
        public static readonly string AllowedTypesMetadataName = "EPiServer.DataAnnotations.AllowedTypesAttribute";

        public static readonly string ContentReferenceMetadataName = "EPiServer.Core.ContentReference";

        public static readonly string ContentReferenceListTypeMetadataName = "System.Collections.Generic.IEnumerable<EPiServer.Core.ContentReference>";
    }
}