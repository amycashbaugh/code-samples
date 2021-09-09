import React, { Component, Suspense, lazy } from "react";
import { withRouter } from "react-router";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import SiteNav from "./SiteNav";
import Unauthorized from "./Unauthorized";
import ReactFormChallenge from "./codechallenge/ReactFormChallenge";
// import Jumbo from "./Jumbo";
// import Content from "./Content";
// import Footer from "./Footer";
import "./App.css";
import { currentUser } from "./services/userService";
//import { Button } from "bootstrap";
import debug from "sabio-debug";

const _logger = debug.extend("App"); //sabio:App
const _loggerPage = _logger.extend("SPA"); //sabio:App:SPA

class App extends Component {
  state = {
    currentUser: {
      roles: ["Admin", "User"],
      userName: "austin",
      email: "austin@example.com",
      avatar: "https://via.placeholder.com/150",
    },
    routeComponents: "",
  };

  routes = [
    {
      path: "/login",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./Login")),
    },
    {
      path: "/register",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./Register")),
    },
    {
      path: "/home",
      exact: true,
      roles: ["User", "Admin"],
      isAnonymous: false,
      component: lazy(() => import("./HomePage")),
    },
    {
      path: "/friends",
      exact: true,
      roles: ["User", "Admin"],
      isAnonymous: false,
      component: lazy(() => import("./Friends")),
    },
    // {
    //   path: "/friends/new",
    //   exact: true,
    //   roles: ["Admin"],
    //   isAnonymous: false,
    //   component: lazy(() => import("./FriendForm")),
    // },
    // {
    //   path: "/friends/:id/edit",
    //   exact: true,
    //   roles: ["Admin"],
    //   isAnonymous: false,
    //   component: lazy(() => import("./FriendForm")),
    // },
    {
      path: "/jobs",
      exact: true,
      roles: ["User", "Admin"],
      isAnonymous: false,
      component: lazy(() => import("./Jobs")),
    },
    {
      path: "/jobs/new",
      exact: true,
      roles: ["Admin"],
      isAnonymous: false,
      component: lazy(() => import("./JobForm")),
    },
    {
      path: "/jobs/:id/edit",
      exact: true,
      roles: ["Admin"],
      isAnonymous: false,
      component: lazy(() => import("./JobForm")),
    },
    {
      path: "/events",
      exact: true,
      roles: ["Admin"],
      isAnonymous: false,
      component: lazy(() => import("./Events")),
    },
    {
      path: "/unauthorized",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./Unauthorized")),
    },
    {
      path: "/friends2/:id/edit",
      exact: true,
      roles: ["Admin"],
      isAnonymous: false,
      component: lazy(() => import("./Friends2")),
    },
    {
      path: "/friends2/new",
      exact: true,
      roles: ["Admin"],
      isAnonymous: false,
      component: lazy(() => import("./Friends2")),
    },
    {
      path: "/login2",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./Login2")),
    },
    {
      path: "/register2",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./Register2")),
    },
    {
      path: "/jobs2/new",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./Jobs2")),
    },
    {
      path: "/jobs2/:id/edit",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./Jobs2")),
    },
    {
      path: "/cars",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./codechallenge/Cars")),
    },
    {
      path: "/forgotpassword/:email",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./EmailConfirmation")),
    },
    {
      path: "/updatepassword",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./PasswordReset")),
    },
    {
      path: "/confirmation/:token",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./EmailConfirmation")),
    },
    {
      path: "/forgotpassword",
      exact: true,
      roles: [],
      isAnonymous: true,
      component: lazy(() => import("./EmailPasswordReset")),
    },
  ];

  componentDidMount = () => {
    this.generateRouteComponents(this.routes);
  };

  mapRoute = (routeData) => {
    let Component = routeData.component;
    return (
      <Route
        key={routeData.path}
        path={routeData.path}
        exact={routeData.exact}
        render={(props) => (
          <Component {...props} currentUser={this.state.currentUser} />
        )}
      />
    );
  };

  generateRouteComponents = (routes) => {
    let filteredRoutes = this.filterRoutes(
      this.state.currentUser.roles,
      routes
    );

    const routeComponents = filteredRoutes.map(this.mapRoute);
    this.setState({ routeComponents: routeComponents });
  };

  filterRoutes(currentUserRoles, routeData) {
    let filteredRoutes = [];

    //if anonymous user
    let anonymousRoutes = routeData.filter(
      (route) => route.isAnonymous === true
    );
    filteredRoutes.push(...anonymousRoutes);
    _logger("ANON:", filteredRoutes);

    //if client is user
    if (currentUserRoles.some((role) => role === "User")) {
      let userRoutes = routeData.filter((route) =>
        route.roles.includes("User")
      );
      filteredRoutes.push(...userRoutes);
      _logger("USER:", filteredRoutes);
    }

    //if client is admin
    if (currentUserRoles.some((role) => role === "Admin")) {
      let adminRoutes = routeData.filter((route) =>
        route.roles.includes("Admin")
      );
      filteredRoutes.push(...adminRoutes);
      _logger("ADMIN:", filteredRoutes);
    }

    return filteredRoutes;
  }

  render() {
    return (
      <React.Fragment>
        <SiteNav></SiteNav>
        <Suspense fallback={<h1>Loading.....</h1>}>
          <Switch location={this.props.location}>
            {this.state.routeComponents}
            <Route path="/">
              <Unauthorized />
            </Route>
          </Switch>
        </Suspense>
      </React.Fragment>
    );
  }
}

export default withRouter(App);
