using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ShopEcommerce.Admin.Permissions;
using ShopEcommerce.Manufacturers;
using ShopEcommerce.Orders;
using ShopEcommerce.Products;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace ShopEcommerce.Admin.Orders
{
    [Authorize(ShopEcommercePermissions.Order.Default, Policy = "AdminOnly")]
    public class OrdersAppService : CrudAppService<
        Order,
        OrderDto,
        Guid,
        PagedResultRequestDto,
        CreateOrderDto,
        CreateOrderDto>, IOrdersAppService
    {
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<Manufacturer, Guid> _manufacturerRepository;
        
        private readonly OrderCodeGenerator _orderCodeGenerator;

        public OrdersAppService(IRepository<Order, Guid> repository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product, Guid> productRepository,
            IRepository<Manufacturer, Guid> manufacturerRepository,
            OrderCodeGenerator orderCodeGenerator
            )
            : base(repository)
        {
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _manufacturerRepository = manufacturerRepository;
            _orderCodeGenerator = orderCodeGenerator;

            GetPolicyName = ShopEcommercePermissions.Order.Default;
            GetListPolicyName = ShopEcommercePermissions.Order.Default;
            CreatePolicyName = ShopEcommercePermissions.Order.Create;
            UpdatePolicyName = ShopEcommercePermissions.Order.Update;
            DeletePolicyName = ShopEcommercePermissions.Order.Delete;
        }

        [Authorize(ShopEcommercePermissions.Order.Delete)]
        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        [Authorize(ShopEcommercePermissions.Order.Default)]
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await Repository.GetListAsync();
            return ObjectMapper.Map<List<Order>, List<OrderDto>>(orders);
        }

        [Authorize(ShopEcommercePermissions.Order.Default)]
        public async Task<PagedResultDto<OrderDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.CustomerName.Contains(input.Keyword));
            query = query.OrderByDescending(x => x.CreationTime);

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<OrderDto>(totalCount, ObjectMapper.Map<List<Order>, List<OrderDto>>(data));
        }

        [Authorize(ShopEcommercePermissions.Order.Create)]
        public async override Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            // Thực hiện logic tạo đơn hàng ở đây

            var order = new Order(); // Đây là đơn hàng tạo mới
                                     // Thực hiện gán các giá trị từ input vào order

            // Lưu đơn hàng vào cơ sở dữ liệu
            var createdOrder = await Repository.InsertAsync(order);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Order, OrderDto>(createdOrder);
        }

        [Authorize(ShopEcommercePermissions.Order.Update)]
        public override async Task<OrderDto> UpdateAsync(Guid id, CreateOrderDto input)
        {
            // Thực hiện logic cập nhật đơn hàng ở đây

            var order = await Repository.GetAsync(id);
            // Thực hiện cập nhật các giá trị từ input vào order

            // Lưu đơn hàng đã cập nhật vào cơ sở dữ liệu
            var updatedOrder = await Repository.UpdateAsync(order);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Order, OrderDto>(updatedOrder);
        }

        [Authorize(ShopEcommercePermissions.Order.Default)]
        public async Task ConfirmOrderAsync(Guid orderId)
        {
            var order = await Repository.GetAsync(orderId);
            if (order != null)
            {
                // Thực hiện các logic cần thiết để xác nhận đơn hàng, ví dụ:
                order.Status = OrderStatus.Confirmed;

                await Repository.UpdateAsync(order);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }


        [Authorize(ShopEcommercePermissions.Order.Default)]
        public async Task<PagedResultDto<ProductSalesDto>> GetProductSalesStatisticsAsync(BaseListFilterDto input)
        {
            var orderItems = await _orderItemRepository.GetListAsync();
            var allProducts = await _productRepository.GetListAsync();
            var allManufacturers = await _manufacturerRepository.GetListAsync();

            var productDictionary = allProducts.ToDictionary(
                product => product.Id,
                product => new { product.Name, product.ManufacturerId, product.ThumbnailPicture });

            var manufacturerDictionary = allManufacturers.ToDictionary(
                manufacturer => manufacturer.Id,
                manufacturer => manufacturer.Name);

            var productSalesQuery = orderItems
                .GroupBy(item => item.ProductId)
                .Select(group =>
                {
                    var productInfo = productDictionary.TryGetValue(group.Key, out var info) ? info : null;
                    return new ProductSalesDto
                    {
                        ProductId = group.Key,
                        ProductName = productInfo?.Name ?? "Unknown Product",
                        ManufacturerName = productInfo != null && manufacturerDictionary.TryGetValue(productInfo.ManufacturerId, out var name) ? name : "Unknown Manufacturer",
                        QuantitySold = group.Sum(item => item.Quantity),
                        TotalRevenue = group.Sum(item => item.Price * item.Quantity),
                        ThumbnailPicture = productInfo?.ThumbnailPicture ?? "default-thumbnail.jpg" // Include the thumbnail URL
                    };
                }).ToList();

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                productSalesQuery = productSalesQuery.Where(sales => sales.ProductName.Contains(input.Keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var totalCount = productSalesQuery.Count;
            var productSales = productSalesQuery
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();

            return new PagedResultDto<ProductSalesDto>(totalCount, productSales);
        }


       



    }
}
