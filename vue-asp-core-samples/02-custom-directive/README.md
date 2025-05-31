# Vue and ASP.NET Core

Demonstrates following:

- Custom directive in vue.js (for SSR based custom tags)
- Separate build/bundle for Custom directive
- Test the Custom Directive alone (HMR) with playground html
- Host the Custom Directive in another Vue app
- Test the Vue app (HMR)
- Use Custom directive in ASP.NET Core app

### Custom Directive

- Custom Directive is for `<hello>` tag
- To test custom directive alone (using HMR)

```
cd hello-directive
npm run dev
```

- create production build/bundle

```
npm run build
```

- the above would create umd and es based files along with source maps to "dist" folder within the "hello-directive" folder
- the bundled files can be directly used in ASP.NET Core or any other SSR service which spits out `<hello>` tags

### Host Custom Directive in another vue app (for testing)

- Ensure prod build of Custom Directive
- To test hosting app:

```
cd hello-app
npm run dev
```

- The above should load/render bundled version of custom directive
- App.vue contains `<hello>` tags
- HMR for host app only refreshes when there is an update in prod build/bundle
- the host uses "es" version of the vue directive

### Testing in ASP.NET Core

- Ensure prod build of Custom Directive
- Copy files from `hello-directive\dist` to `wwwroot\js`
- .cshtml contains `<hello>` tags
- Execute/run the solution to load/render the bundled version of custom directive
- HMR doesn't work in this case
- ASP.NET Core uses "umd" version of the vue directive
