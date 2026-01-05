using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Web
{
    public class MvcLocalizationConfiguration(IStringLocalizerFactory factory) : IConfigureOptions<MvcOptions>
    {
        private readonly IStringLocalizer _localizer = factory.Create("ValidationMessages", typeof(Program).Assembly.GetName().Name!);

        public void Configure(MvcOptions options)
        {
            options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
                name => _localizer["GenericNumberError"]);

            options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
                value => _localizer["GenericInvalidError"]);
        }
    }
}
