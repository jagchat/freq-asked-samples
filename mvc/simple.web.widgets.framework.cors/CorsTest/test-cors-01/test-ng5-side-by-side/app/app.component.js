"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
//Class & Style Binding
var AppComponent = /** @class */ (function () {
    function AppComponent() {
        this.a = "";
        this.b = "";
        this.sum = 0;
        this.isBold = true;
        this.isItalic = true;
    }
    AppComponent.prototype.ngOnInit = function () {
        this.setStyle();
    };
    AppComponent.prototype.applyChanges = function () {
        this.sum = +this.a + +this.b;
        this.setStyle();
    };
    AppComponent.prototype.setStyle = function () {
        //for ngStyle
        this.stylesToApply = {
            'font-weight': this.isBold ? 'bold' : 'normal',
            'font-style': this.isItalic ? 'italic' : 'normal',
            'color': (this.sum > 100) ? 'red' : 'black'
        };
        //for ngClass
        this.classesToApply = {
            'bold-text': this.isBold,
            'italic-text': this.isItalic,
        };
    };
    AppComponent = __decorate([
        core_1.Component({
            selector: 'app',
            moduleId: module.id,
            templateUrl: 'app.component.html',
            styles: [
                "\n        .bold-text{\n            font-weight: bold;\n        }\n\n        .italic-text{\n            font-style: italic;\n        }\n\n        "
            ]
        }),
        __metadata("design:paramtypes", [])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map