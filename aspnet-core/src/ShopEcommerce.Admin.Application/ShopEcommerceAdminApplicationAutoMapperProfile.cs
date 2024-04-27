using AutoMapper;
using ShopEcommerce.ProductCategories;
using ShopEcommerce.Admin.ProductCategories;
using ShopEcommerce.Admin.Products;
using ShopEcommerce.Products;
using ShopEcommerce.Manufacturers;
using ShopEcommerce.Admin.Manufacturers;
using ShopEcommerce.ProductAttributes;
using ShopEcommerce.Admin.ProductAttributes;

namespace ShopEcommerce.Admin;

public class ShopEcommerceAdminApplicationAutoMapperProfile : Profile
{
    public ShopEcommerceAdminApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        //Product Category
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategory, ProductCategoryInListDto>();
        CreateMap<CreateUpdateProductCategoryDto, ProductCategory>();

        //Product
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductInListDto>();
        CreateMap<CreateUpdateProductDto, Product>();

        //Manufacturer
        CreateMap<Manufacturer, ManufacturerDto>();
        CreateMap<Manufacturer, ManufacturerInListDto>();
        CreateMap<CreateUpdateManufacturerDto, Manufacturer>();

        //Product attribute
        CreateMap<ProductAttribute, ProductAttributeDto>();
        CreateMap<ProductAttribute, ProductAttributeInListDto>();
        CreateMap<CreateUpdateProductAttributeDto, ProductAttribute>();
    }
}