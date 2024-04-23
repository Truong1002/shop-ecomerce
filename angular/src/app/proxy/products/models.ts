import type { ProductType } from '../shop-ecommerce/products/product-type.enum';
import type { EntityDto } from '@abp/ng.core';
import type { BaseListFilterDto } from '../models';

export interface CreateUpdateProductDto {
  manufacturerId?: string;
  name?: string;
  code?: string;
  slug?: string;
  productType: ProductType;
  sku?: string;
  sortOrder: number;
  visibility: boolean;
  isActive: boolean;
  sellPrice: number;
  categoryId?: string;
  seoMetaDescription?: string;
  description?: string;
  stockQuantity: number;
  thumbnailPicture?: string;
}

export interface ProductDto {
  manufacturerId?: string;
  name?: string;
  code?: string;
  slug?: string;
  productType: ProductType;
  sku?: string;
  sortOrder: number;
  visibility: boolean;
  isActive: boolean;
  categoryId?: string;
  seoMetaDescription?: string;
  description?: string;
  stockQuantity: number;
  thumbnailPicture?: string;
  sellPrice: number;
  id?: string;
  categoryName?: string;
  categorySlug?: string;
}

export interface ProductInListDto extends EntityDto<string> {
  manufacturerId?: string;
  name?: string;
  code?: string;
  slug?: string;
  productType: ProductType;
  sku?: string;
  sortOrder: number;
  visibility: boolean;
  isActive: boolean;
  categoryId?: string;
  thumbnailPicture?: string;
  categoryName?: string;
  categorySlug?: string;
}

export interface ProductListFilterDto extends BaseListFilterDto {
  categoryId?: string;
}
