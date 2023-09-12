"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.Home = void 0;
const react_1 = __importStar(require("react"));
class Home extends react_1.Component {
    render() {
        return (react_1.default.createElement("div", null,
            react_1.default.createElement("h1", null, "Hello, world!"),
            react_1.default.createElement("p", null, "Welcome to your new single-page application, built with:"),
            react_1.default.createElement("ul", null,
                react_1.default.createElement("li", null,
                    react_1.default.createElement("a", { href: 'https://get.asp.net/' }, "ASP.NET Core"),
                    " and ",
                    react_1.default.createElement("a", { href: 'https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx' }, "C#"),
                    " for cross-platform server-side code"),
                react_1.default.createElement("li", null,
                    react_1.default.createElement("a", { href: 'https://facebook.github.io/react/' }, "React"),
                    " for client-side code"),
                react_1.default.createElement("li", null,
                    react_1.default.createElement("a", { href: 'http://getbootstrap.com/' }, "Bootstrap"),
                    " for layout and styling")),
            react_1.default.createElement("p", null, "To help you get started, we have also set up:"),
            react_1.default.createElement("ul", null,
                react_1.default.createElement("li", null,
                    react_1.default.createElement("strong", null, "Client-side navigation"),
                    ". For example, click ",
                    react_1.default.createElement("em", null, "Counter"),
                    " then ",
                    react_1.default.createElement("em", null, "Back"),
                    " to return here."),
                react_1.default.createElement("li", null,
                    react_1.default.createElement("strong", null, "Development server integration"),
                    ". In development mode, the development server from ",
                    react_1.default.createElement("code", null, "create-react-app"),
                    " runs in the background automatically, so your client-side resources are dynamically built on demand and the page refreshes when you modify any file."),
                react_1.default.createElement("li", null,
                    react_1.default.createElement("strong", null, "Efficient production builds"),
                    ". In production mode, development-time features are disabled, and your ",
                    react_1.default.createElement("code", null, "dotnet publish"),
                    " configuration produces minified, efficiently bundled JavaScript files.")),
            react_1.default.createElement("p", null,
                "The ",
                react_1.default.createElement("code", null, "ClientApp"),
                " subdirectory is a standard React application based on the ",
                react_1.default.createElement("code", null, "create-react-app"),
                " template. If you open a command prompt in that directory, you can run ",
                react_1.default.createElement("code", null, "npm"),
                " commands such as ",
                react_1.default.createElement("code", null, "npm test"),
                " or ",
                react_1.default.createElement("code", null, "npm install"),
                ".")));
    }
}
exports.Home = Home;
Home.displayName = Home.name;
