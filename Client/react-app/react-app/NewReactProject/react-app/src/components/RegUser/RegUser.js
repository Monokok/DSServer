import { Modal } from "antd";
import React, { useState } from "react";
import './RegUser.css';
//import { useNavigate } from "react-router-dom";

const RegUser = ({ user, setUser, getUser }) => {
  /*Если пользователь успешно зарегистрирован result = true (для вывода сообщения об успешной регистрации) */
  const [result, setResult] = React.useState(false);
  /*Поле для вывода ошибок при регистрации: не указан email и пр.*/
  const [errorMessages, setErrorMessages] = useState([]);
  //const navigate = useNavigate();

  //функция регистрации пользователя на основе данных указанных в input'ах
  const regUser = async (event) => {
    try {
      event.preventDefault();
      var {
        email,
        password,
        passwordConfirm,
        firstName,
        middleName,
        lastName,
      } = document.forms[0];
      // console.log(email.value, password.value)
      const requestOptions = {
        method: "POST",

        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          email: email.value,
          password: password.value,
          passwordConfirm: passwordConfirm.value,
          firstName: firstName.value,
          middleName: middleName.value,
          lastName: lastName.value,
        }),
      };

      const response = await fetch("api/account/register", requestOptions);
      const data = await response.json();

      switch (response.status) {
        case 200:
          setResult(true);
          console.log("Data:", data);
          Modal.success({
            title: "Регистрация успешна!",
            content: "Новый пользователь зарегистрирован в системе",
          });
          break;
        default:
          setErrorMessages(data.error);
          setResult(false);
          console.log("Data:", data.error);
          break;
      }
    } catch (error) {
      console.error("Ошибка при регистрации:", error);
      setResult(false);
    }
  };
  //вывод деталей ошибок регистрации из массива
  const renderErrorMessage = () =>
    errorMessages.map((error, index) => <div style={{ color: '#d9534f', marginTop: '10px' }} key={index}>{error}</div>);

  return (
    <div className="reg-user-container">
      {result === true ? (
        <h3 className="reg-user-title">
          Пользователь {user.firstName} успешно зарегистрирован в системе!
        </h3>
      ) : (
        ""
      )}
      <>
        <h3>Зарегистрироваться</h3>
        <form className="reg-user-form" onSubmit={regUser}>
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
          <input
            type="text"
            name="passwordConfirm"
            placeholder="Подтвердите пароль"
          />
          <br />
          <button type="submit">Зарегистрироваться</button>
        </form>
        {renderErrorMessage()}
      </>
    </div>
  );
};
export default RegUser;
