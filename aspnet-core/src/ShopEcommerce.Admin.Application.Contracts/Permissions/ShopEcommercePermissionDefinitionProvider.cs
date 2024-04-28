using ShopEcommerce.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace ShopEcommerce.Admin.Permissions;

public class ShopEcommercePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        //Catalog
        var catalogGroup = context.AddGroup(ShopEcommercePermissions.CatalogGroupName, L("Permission:Catalog"));

        //Add product
        var productPermission = catalogGroup.AddPermission(ShopEcommercePermissions.Product.Default, L("Permission:Catalog.Product"));
        productPermission.AddChild(ShopEcommercePermissions.Product.Create, L("Permission:Catalog.Product.Create"));
        productPermission.AddChild(ShopEcommercePermissions.Product.Update, L("Permission:Catalog.Product.Update"));
        productPermission.AddChild(ShopEcommercePermissions.Product.Delete, L("Permission:Catalog.Product.Delete"));
        productPermission.AddChild(ShopEcommercePermissions.Product.AttributeManage, L("Permission:Catalog.Product.AttributeManage"));

        //Add attribute
        var attributePermission = catalogGroup.AddPermission(ShopEcommercePermissions.Attribute.Default, L("Permission:Catalog.Attribute"));
        attributePermission.AddChild(ShopEcommercePermissions.Attribute.Create, L("Permission:Catalog.Attribute.Create"));
        attributePermission.AddChild(ShopEcommercePermissions.Attribute.Update, L("Permission:Catalog.Attribute.Update"));
        attributePermission.AddChild(ShopEcommercePermissions.Attribute.Delete, L("Permission:Catalog.Attribute.Delete"));
        //Add category
        var categoryPermission = catalogGroup.AddPermission(ShopEcommercePermissions.Category.Default, L("Permission:Catalog.Category"));
        categoryPermission.AddChild(ShopEcommercePermissions.Category.Create, L("Permission:Catalog.Category.Create"));
        categoryPermission.AddChild(ShopEcommercePermissions.Category.Update, L("Permission:Catalog.Category.Update"));
        categoryPermission.AddChild(ShopEcommercePermissions.Category.Delete, L("Permission:Catalog.Category.Delete"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ShopEcommerceResource>(name);
    }
}