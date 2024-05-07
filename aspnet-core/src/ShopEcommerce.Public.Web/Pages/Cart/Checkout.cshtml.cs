using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopEcommerce.Public.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ShopEcommerce.Public.Orders;
using ShopEcommerce.Public.Web.Extensions;
using ShopEcommerce.Public.Web.Models;
using Volo.Abp.TextTemplating;
using Volo.Abp.Emailing;
using ShopEcommerce.Emailing;
using System.Security.Claims;

namespace ShopEcommerce.Public.Web.Pages.Cart
{
    public class CheckoutModel : PageModel
    {
        private readonly IOrdersAppService _ordersAppService;
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        public CheckoutModel(IOrdersAppService ordersAppService, IEmailSender emailSender,
           ITemplateRenderer templateRenderer)
        {
            _ordersAppService = ordersAppService;
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
        }
        public List<CartItem> CartItems { get; set; }

        public bool? CreateStatus { set; get; }

        [BindProperty]
        public OrderDto Order { set; get; }

        public void OnGet()
        {
            
            CartItems = GetCartItems();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                
            }

            
            var cartItems = new List<OrderItemDto>();
            double total = 0; // Initialize total amount
            string productDetails = ""; // Initialize product details string
            foreach (var item in GetCartItems())
            {
                cartItems.Add(new OrderItemDto()
                {
                    Price = item.Product.SellPrice,
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity
                });
                total += item.Product.SellPrice * item.Quantity;
                productDetails += $"{item.Product.Name} (Số lượng: {item.Quantity} x {item.Product.SellPrice.ToString("N0")})\n";
            }
            Guid? currentUserId = User.Identity.IsAuthenticated ? User.GetUserId() : null;
            if(currentUserId == null)
            {
                return Redirect("/login.html");
            }
            
            var order = await _ordersAppService.CreateAsync(new CreateOrderDto()
            {               
                CustomerName = Order.CustomerName,
                CustomerAddress = Order.CustomerAddress,
                CustomerPhoneNumber = Order.CustomerPhoneNumber,
                Items = cartItems,
                CustomerUserId = currentUserId
            });
            CartItems = GetCartItems();

            if (order != null)
            {
                /*if (User.Identity.IsAuthenticated)
                {
                    var email = User.GetSpecificClaim(ClaimTypes.Email);
                    var totalAsString = total.ToString("N0");
                    var emailBody = await _templateRenderer.RenderAsync(
                        EmailTemplates.CreateOrderEmail,
                        new
                        {
                            message = $"{productDetails}\nTổng tiền là: {totalAsString}",
                        });
                    await _emailSender.SendAsync(email, "Tạo đơn hàng thành công", emailBody);
                    HttpContext.Session.Remove(ShopEcommerceConsts.Cart);

                }*/
                CreateStatus = true;
            }
                
            else
                CreateStatus = false;

            return Page();
        }

        private List<CartItem> GetCartItems()
        {
            var cart = HttpContext.Session.GetString(ShopEcommerceConsts.Cart);
            var productCarts = new Dictionary<string, CartItem>();
            if (cart != null)
            {
                productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
            }
            return productCarts.Values.ToList();
        }

    }
}