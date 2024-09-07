import React from "react";
import { useNavigate } from "react-router-dom";
import { Modal } from "antd";
import { useEffect } from "react";
import { useState } from "react";

const LogOff = ({ user, setUser }) => {
  const [open, setOpen] = useState(false); //отслеживать открытие и закрытие модального окна. состояние и сеттер
  const navigate = useNavigate();
  const showModal = () => {
    setOpen(true);
  };
  useEffect(() => {
    showModal();
  }, []);

  //Функция выхода
  const logOff = async (event) => {
    event.preventDefault();
    const requestOptions = {
      method: "POST",
    };

    //return
    try {
      await fetch("api/account/logoff", requestOptions).then((response) => {
        response.status === 200 &&
          setUser({ isAuthenticated: false, userName: "Гость" }) &&
          navigate("/");
        response.status === 401 &&
          setUser({ isAuthenticated: false, userName: "Гость" }) &&
          navigate("LogIn");
      }).then(console.log("Выход успешен"));
    } catch (error) {
      console.error("Ошибка при выходе logoff():", error);
      return false; // Возвращаем false в случае ошибки
    }
  };

  //при закрытии модального окна на Cancel
  const handleCancel = () => {
    console.log("Clicked cancel button");
    setOpen(false);
    navigate("/");
  };
  if (user.isAuthenticated)
    return (
      <>
        <Modal className="modal-footer"
          title="Выход из аккаунта"
          open={open}
          onOk={logOff}
          onCancel={handleCancel}
          okText="Выход"
          cancelText="Отмена"
        >
          <p>Выполнить выход?</p>
          
        </Modal>
      </>
    );
};
export default LogOff;
