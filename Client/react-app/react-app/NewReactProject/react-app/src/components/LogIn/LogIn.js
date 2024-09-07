import React, { useState } from "react";
import { Button, Checkbox, Form, Input } from "antd";

const LogIn = ({ user, setUser }) => {
  const [errorMessages, setErrorMessages] = useState([]); //для обработки деталей неуспешного входа

  //функция входа
  const logIn = async (formValues) => {
    try {
      console.log("Success:", formValues);
      const requestOptions = {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          // email: email.value,
          // password: password.value,
          Email: formValues.username,
          Password: formValues.password,
          RememberMe: formValues.remember,
        }),
      };
      const response = await fetch("api/account/login", requestOptions);
      const a = await response.json();
      const tempUser = a.user;
      switch (response.status) {
        case 401:
          //navige("/login");
          console.log("Ошибка при попытке входа, status.code:", response.status);
          break;
        case 200:
          setUser(tempUser); //получаем в респонсе помимо user ещё данные. resp {user{}, others}
          break;
        default:
          setErrorMessages(a.error);
          break;
      }
      console.log(response.error);
      return;
    } catch (error) {
      console.error("Произошла ошибка при попытке входа:", error);
      setErrorMessages(["Произошла ошибка при попытке входа"]);
    }
  };

  //вывод деталей ошибки входа
  const renderErrorMessage = () =>
    errorMessages.map((error, index) => <div key={index}>{error}</div>);
  return (
    <>
      {console.log("LogIn user:", user)}
      {user.isAuthenticated ? (
        <h3>Пользователь {user.userName} успешно вошел в систему</h3>
      ) : (
        <>
          <h3>Вход</h3>
          <Form
            onFinish={logIn}
            name="basic"
            labelCol={{ span: 8 }}
            wrapperCol={{ span: 16 }}
            style={{ maxWidth: 600 }}
            initialValues={{ remember: true }}
            onFinishFailed={renderErrorMessage}
            autoComplete="off"
          >
            <Form.Item
              label="Username"
              name="username"
              rules={[
                { required: true, message: "Please input your username!" },
              ]}
            >
              <Input />
            </Form.Item>
            <Form.Item
              label="Password"
              name="password"
              rules={[
                { required: true, message: "Please input your password!" },
              ]}
            >
              <Input.Password />
            </Form.Item>
            <Form.Item
              name="remember"
              valuePropName="checked"
              wrapperCol={{ offset: 8, span: 16 }}
            >
              <Checkbox>Запомнить меня</Checkbox>
              {renderErrorMessage()}
            </Form.Item>
            <Form.Item wrapperCol={{ offset: 8, span: 16 }}>
              <Button type="primary" htmlType="submit">
                Войти
              </Button>
            </Form.Item>
          </Form>

          {renderErrorMessage()}
        </>
      )}
    </>
  );
};
export default LogIn;
