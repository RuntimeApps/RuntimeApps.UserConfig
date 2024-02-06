# RuntimeApps.UserConfig

Many application need a part to save a retrive user settings. This part is usally common in many applications. RuntimeApps.UserConfig provide a place to set and get the user settings. Also, a default config can be set for all users. Default configs can be changed by admin.

## Install

Their is 3 package to install based on your need:

### RuntimeApps.UserConfig

This package is the main package and should be installed everywhere you want to use `IUserConfigService`.

### RuntimeApps.UserConfig.EntityFrameworkCore

This package is the implementation of `IUserConfigStore` with Entity Framework Core which manage DB part of user configs. If you want to implement your own store part, it is not required, otherwise it should be added to your DB layer.

### RuntimeApps.UserConfig.AspNet

This package has an implementation of APIs for user configs. It should be added to your ASP.Net project if you want to use user config APIs.

## Interfaces

If you want to change the behavier of this package, you can reimplment this interfaces and added to DI:

* **IUserConfigService**: The main implementation of user configs
* **IUserConfigValidation**: Validate key and value of input models. Implement if you need cosutom validations.
* **IUserConfigCache**: Cache the user configs in memory
* **IUserConfigStore**: Store of user config
* **IUserConfigValueSerializer**: The value serializer
* **DbUserConfigEntityConfiguration**: Entity framework configuration for DbUserConfigModel

## License

Distributed under the MIT License. See [LICENSE](./LICENSE) for more information.
