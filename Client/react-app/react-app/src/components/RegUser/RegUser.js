import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
const RegUser = ({ user, setUser, getUser }) => {
  const [errorMessages, setErrorMessages] = useState([]);
  const navigate = useNavigate();
  const regUser = async (event) => {
    event.preventDefault();
    var { email, password, passwordConfirm, firstName, middleName, lastName } = document.forms[0];
    // console.log(email.value, password.value)
    const requestOptions = {
      method: "POST",

      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        email: email.value,
        password: password.value,
        passwordConfirm : passwordConfirm.value,
        firstName: firstName.value,
        middleName: middleName.value,
        lastName: lastName.value,
      }),
    };
    return await fetch("https://localhost:7230/api/account/register", requestOptions)
      .then((response) => {
        // console.log(response.status)
        response.status !== 200 && setUser({ isAuthenticated: false, userName: "" });
        return response.json();
      })
      .then(
        (data) => {
          console.log("Data:", data);
          if (
            typeof data !== "undefined" &&
            typeof data.email !== "undefined"
          ) {
            getUser();
            setUser({ isAuthenticated: true, userName: ""});
            navigate("/");
          }
          typeof data !== "undefined" &&
            typeof data.error !== "undefined" &&
            setErrorMessages(data.error);
        },
        (error) => {
          console.log(error);
        }
      );
  };
  const renderErrorMessage = () =>
    errorMessages.map((error, index) => <div key={index}>{error}</div>);
  return (
    <>
      { user.isAuthenticated ? (
        <h3>Пользователь {user.firstName} успешно зарегистрирован в системе</h3>
      ) : ("")}
        <>
          <h3>Зарегистрироваться</h3>
          <form onSubmit={regUser}>
            <label>Почта </label>
            <input type="text" name="email" placeholder="Логин" />
            <br />
            <label>Имя </label>
            <input type="text" name="firstName" placeholder="Имя" />
            <br />
            <label>Отчество </label>
            <input type="text" name="middleName" placeholder="Отчество" />
            <br />
            <label>Фамилия </label>
            <input type="text" name="lastName" placeholder="Фамилия" />
            <br />
            <label>Пароль </label>
            <input type="text" name="password" placeholder="Пароль" />
            <br />
            <label>Подтвердите пароль </label>
            <input type="text" name="passwordConfirm" placeholder="Подтвердите пароль" />
            <br />
            <button type="submit">Зарегистрироваться</button>
          </form>
          {renderErrorMessage()}
        </>
    </>
  );
};
export default RegUser;