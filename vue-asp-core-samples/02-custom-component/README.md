# Vue and ASP.NET Core

Demonstrates following:

- Custom component in vue.js (for SSR based custom tags)
- Separate build/bundle for Custom component
- Test the Custom component alone (HMR) with playground html
- Host the Custom component in another Vue app
- Test the Vue app (HMR)
- Use Custom component in ASP.NET Core app

### Custom component

- Custom component is for `<hello>` tag
- To test custom component alone (using HMR)

```
cd hello-component
npm run dev
```

- create production build/bundle

```
npm run build
```

- the above would create umd and es based files along with source maps to "dist" folder within the "hello-component" folder
- the bundled files can be directly used in ASP.NET Core or any other SSR service which spits out `<hello>` tags

### Host Custom component in another vue app (for testing)

- Ensure prod build of Custom component
- To test hosting app:

```
cd hello-app
npm run dev
```

- The above should load/render bundled version of custom component
- App.vue contains `<hello>` tags
- HMR for host app only refreshes when there is an update in prod build/bundle
- the host uses "es" version of the vue component

### Testing in ASP.NET Core

- Ensure prod build of Custom component
- Copy files from `hello-component\dist` to `wwwroot\js`
- .cshtml contains `<hello>` tags
- Execute/run the solution to load/render the bundled version of custom component
- HMR doesn't work in this case
- ASP.NET Core uses "umd" version of the vue component
