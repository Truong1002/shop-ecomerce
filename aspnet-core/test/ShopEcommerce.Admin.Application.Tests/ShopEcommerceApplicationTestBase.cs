using Volo.Abp.Modularity;

namespace ShopEcommerce.Admin;

public abstract class ShopEcommerceApplicationTestBase<TStartupModule> : ShopEcommerceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
