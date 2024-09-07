import React from "react";
import { Outlet, Link } from "react-router-dom";
const Layout = ({ user }) => {
  return (
    <>
      <div>
        {user.isAuthenticated ? (
          <h4>Пользователь: {user.userName}</h4>
        ) : (
          <h4>Пользователь: Гость</h4>
        )}
      </div>
      <nav>
        <Link to="/">Главная</Link> <span> </span>
        <Link to="/lessons">Занятия</Link> <span> </span>
        <Link to="/login">Вход</Link> <span> </span>
        <Link to="/logoff">Выход</Link> <span> </span>
        <Link to="/regUser">Зарегистрироваться</Link>
      </nav>
      <Outlet />
    </>
  );
};
export default Layout;