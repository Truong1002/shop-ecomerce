using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopEcommerce.Public.Orders;
using ShopEcommerce.Public.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopEcommerce.Public.Web.Pages.Cart
{
    public class MyorderModel : PageModel
    {
        private readonly IOrdersAppService _ordersAppService;

        public MyorderModel(IOrdersAppService ordersAppService)
        {
            _ordersAppService = ordersAppService;
        }

        public List<OrderDto> Orders { get; set; }

        public async Task OnGetAsync()
        {
            Guid? currentUserId = User.Identity.IsAuthenticated ? User.GetUserId() : null;

            if (currentUserId.HasValue)
            {
                Orders = await _ordersAppService.GetCustomerOrdersAsync(currentUserId.Value);
            }
            else
            {
                // Handle the case where the user is not authenticated or the ID is invalid
                Orders = new List<OrderDto>();
            }
        }
    }
}
