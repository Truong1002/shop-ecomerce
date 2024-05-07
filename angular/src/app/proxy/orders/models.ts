import type { EntityDto } from '@abp/ng.core';
import type { OrderStatus } from '../shop-ecommerce/orders/order-status.enum';
import type { PaymentMethod } from '../shop-ecommerce/orders/payment-method.enum';

export interface CreateOrderDto {
  customerName?: string;
  customerPhoneNumber?: string;
  customerAddress?: string;
  customerUserId?: string;
  items: OrderItemDto[];
}

export interface OrderDto extends EntityDto<string> {
  code?: string;
  status: OrderStatus;
  paymentMethod: PaymentMethod;
  shippingFee: number;
  tax: number;
  total: number;
  subtotal: number;
  discount: number;
  grandTotal: number;
  customerName?: string;
  customerPhoneNumber?: string;
  customerAddress?: string;
  customerUserId?: string;
}

export interface OrderItemDto extends EntityDto {
  orderId?: string;
  productId?: string;
  sku?: string;
  quantity: number;
  price: number;
}
