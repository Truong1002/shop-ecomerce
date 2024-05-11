using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopEcommerce.Orders;
using ShopEcommerce.Products;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ShopEcommerce.Public.Orders
{
    public class OrdersAppService : CrudAppService<
        Order,
        OrderDto,
        Guid,
        PagedResultRequestDto, CreateOrderDto, CreateOrderDto>, IOrdersAppService
    {
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly OrderCodeGenerator _orderCodeGenerator;
        private readonly IRepository<Product, Guid> _productRepository;
        public OrdersAppService(IRepository<Order, Guid> repository,
            OrderCodeGenerator orderCodeGenerator,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product, Guid> productRepository)
            : base(repository)
        {
            _orderItemRepository = orderItemRepository;
            _orderCodeGenerator = orderCodeGenerator;
            _productRepository = productRepository;
        }

        public override async Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            var subTotal = input.Items.Sum(x => x.Quantity * x.Price);
            var orderId = Guid.NewGuid();
            var order = new Order(orderId)
            {
                Code = await _orderCodeGenerator.GenerateAsync(),
                CustomerAddress = input.CustomerAddress,
                CustomerName = input.CustomerName,
                CustomerPhoneNumber = input.CustomerPhoneNumber,
                ShippingFee = 0,
                CustomerUserId = input.CustomerUserId,
                Tax = 0,
                Subtotal = subTotal,
                GrandTotal = subTotal,
                Discount = 0,
                PaymentMethod = PaymentMethod.COD,
                Total = subTotal,
                Status = OrderStatus.New

            };
            var items = new List<OrderItem>();
            foreach (var item in input.Items)
            {
                var product = await _productRepository.GetAsync(item.ProductId);
                items.Add(new OrderItem()
                {
                    OrderId = orderId,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SKU = product.SKU
                });
            }
            await _orderItemRepository.InsertManyAsync(items);

            var result = await Repository.InsertAsync(order);


            return ObjectMapper.Map<Order, OrderDto>(result);
        }

        public async Task<List<OrderDto>> GetCustomerOrdersAsync(Guid customerUserId)
        {
            // Query orders based on CustomerUserId and sort by CreationTime descending
            var orders = await Repository.GetListAsync(order => order.CustomerUserId == customerUserId);

            // Sort orders by CreationTime descending (latest first)
            var sortedOrders = orders.OrderByDescending(order => order.CreationTime);

            // Transform the sorted list into OrderDto
            return sortedOrders.Select(order => new OrderDto
            {
                Id = order.Id,
                Code = order.Code,
                CustomerAddress = order.CustomerAddress,
                CustomerName = order.CustomerName,
                CustomerPhoneNumber = order.CustomerPhoneNumber,
                OrderDate = order.CreationTime,
                Status = order.Status,
                Total = order.Total
            }).ToList();
        }

        public async Task<OrderDto> UpdateAsync(Guid id, OrderDto input)
        {
            var existingOrder = await Repository.GetAsync(id);
            if (existingOrder == null)
            {
                throw new KeyNotFoundException("Không tìm thấy đơn hàng với ID đã cho.");
            }

            // Cập nhật thuộc tính đơn hàng
            existingOrder.CustomerAddress = input.CustomerAddress;
            existingOrder.CustomerName = input.CustomerName;
            existingOrder.CustomerPhoneNumber = input.CustomerPhoneNumber;
            existingOrder.Status = input.Status;
            existingOrder.PaymentMethod = input.PaymentMethod;
            existingOrder.ShippingFee = input.ShippingFee;
            existingOrder.Tax = input.Tax;
            existingOrder.Total = input.Total;
            existingOrder.Subtotal = input.Subtotal;
            existingOrder.Discount = input.Discount;
            existingOrder.GrandTotal = input.GrandTotal;

            // Lưu thay đổi
            await Repository.UpdateAsync(existingOrder);

            return ObjectMapper.Map<Order, OrderDto>(existingOrder);
        }


    }
}