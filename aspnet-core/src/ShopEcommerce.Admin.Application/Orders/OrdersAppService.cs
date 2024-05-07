using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ShopEcommerce.Admin.Permissions;
using ShopEcommerce.Orders;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ShopEcommerce.Admin.Orders
{
    public class OrdersAppService : CrudAppService<
        Order,
        OrderDto,
        Guid,
        PagedResultRequestDto,
        CreateOrderDto,
        CreateOrderDto>, IOrdersAppService
    {
        public OrdersAppService(IRepository<Order, Guid> repository)
            : base(repository)
        {
            
        }

        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await Repository.GetListAsync();
            return ObjectMapper.Map<List<Order>, List<OrderDto>>(orders);
        }

        public async Task<PagedResultDto<OrderDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.CustomerName.Contains(input.Keyword));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<OrderDto>(totalCount, ObjectMapper.Map<List<Order>, List<OrderDto>>(data));
        }

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
    }
}
