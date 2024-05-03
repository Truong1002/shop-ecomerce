using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopEcommerce.Public.Products;  // Make sure this using directive is correct
using System;
using System.Threading.Tasks;

namespace ShopEcommerce.Public.Web.Pages.Products
{
    public class SearchModel : PageModel
    {
        private readonly IProductsAppService _productAppService;
        public PagedResult<ProductInListDto> Products { get; set; }

        public SearchModel(IProductsAppService productAppService)
        {
            _productAppService = productAppService;
        }

        public async Task OnGetAsync(string keyword, Guid? categoryId)
        {
            var input = new ProductListFilterDto
            {
                Keyword = keyword,
                CategoryId = categoryId,
                CurrentPage = 1,
                PageSize = 10
            };

            Products = await _productAppService.GetListFilterAsync(input);
        }
    }
}