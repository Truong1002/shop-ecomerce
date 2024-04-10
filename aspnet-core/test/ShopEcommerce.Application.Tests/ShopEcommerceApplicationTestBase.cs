using Volo.Abp.Modularity;

namespace ShopEcommerce;

public abstract class ShopEcommerceApplicationTestBase<TStartupModule> : ShopEcommerceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
