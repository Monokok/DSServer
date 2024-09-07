import React from "react";
import { Outlet, Link } from "react-router-dom";

import { Layout as LayoutAntd, Menu } from "antd";
const { Header, Content, Footer } = LayoutAntd;

const items = [
  {
    label: <Link to={"/"}>Главная</Link>,
    key: "1",
  },
  {
    label: <Link to={"/lessons"}>Занятия</Link>,
    key: "2",
  },
  {
    label: <Link to={"/login"}>Вход</Link>,
    key: "3",
  },
  {
    label: <Link to={"/reguser"}>Регистрация</Link>,
    key: "4",
  },
  {
    label: <Link to={"/logoff"}>Выход</Link>,
    key: "5",
  },
];

const Layout = ({ user }) => {
  var roleName = "";
  if (user.userRole === "user") {
    roleName = "Обучающийся";
  } else if (user.userRole === "teacher") {
    roleName = "Преподаватель";
  } else roleName = "Пользователь";

  return (
    <>
      {
        <LayoutAntd>
          <Header
            style={{ position: "sticky", top: 0, zIndex: 1, width: "100%" }}
          >
            <div
              style={{
                float: "right",
                color: "rgba(255, 255, 255, 0.65)",
              }}
            >
              {user.isAuthenticated ? (
                <strong>
                  {roleName}
                  {" "}
                  {user.first_name +
                    " " +
                    user.middle_name +
                    " " +
                    user.last_name + " "}
                    
                </strong>
                
              ) : (
                <strong>Гость</strong>
              )}
            </div>
            <Menu
              theme="dark"
              mode="horizontal"
              items={items}
              className="menu"
            />
          </Header>
          <Content className="site-layout" style={{ padding: "0 50px" }}>
            <Outlet />
          </Content>
          <Footer style={{ textAlign: "center" }}>Driving School ©2024</Footer>
        </LayoutAntd>
}
    </>
  );
};
export default Layout;



    //     /* {user.isAuthenticated ? (
    //     <div>
    //       <h3>
    //         Пользователь:{" "}
    //         {user.first_name + " " + user.middle_name + " " + user.last_name}
    //       </h3>
    //       <h4>Тип: {roleName}</h4>
    //     </div>
    //   ) : (
    //     <h4>Пользователь: Гость</h4>
    //   )} */
    // }
    // {/* {console.log("layout.js user:", user.first_name)} */}
    // {/* 
    // <nav>
    //   <Link to="/">Главная</Link> <span> </span>
    //   <Link to="/lessons">Занятия</Link> <span> </span>
    //   <Link to="/login">Вход</Link> <span> </span>
    //   <Link to="/logoff">Выход</Link> <span> </span>
    //   <Link to="/regUser">Зарегистрироваться</Link>
    // </nav>
    // <Outlet /> */