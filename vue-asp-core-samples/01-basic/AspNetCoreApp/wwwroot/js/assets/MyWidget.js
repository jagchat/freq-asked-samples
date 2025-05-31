import { createElementBlock as c, openBlock as s } from "vue";
const a = (e, o) => {
  const t = e.__vccOpts || e;
  for (const [n, r] of o)
    t[n] = r;
  return t;
}, p = {
  name: "MyWidget"
};
function _(e, o, t, n, r, l) {
  return s(), c("div", null, "Hello from Vue Component!");
}
const d = /* @__PURE__ */ a(p, [["render", _]]);
export {
  d as default
};
