import React, { useState } from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import LessonCreate from "./components/lessonCreate/lessonCreate";
import Lesson from "./components/lesson/lesson";
import Layout from "./components/layout/layout";
import LogIn from "./components/LogIn/LogIn";
import LogOff from "./components/LogOff/LogOff";
import RegUser from "./components/RegUser/RegUser";
import { useEffect } from "react";
import { Typography, Row, Col } from "antd";
import ErrorBoundary from "./ErrorBoundary/ErrorBoundary";

const { Title, Paragraph } = Typography;

const App = () => {
  const [lessons, setLessons] = useState([]); //уроки и сеттер
  const [teachers, setTeachers] = useState([]); //преподаватели и сеттер
  const [user, setUser] = useState({ isAuthenticated: false }); //текущий пользователь

  /*Функция получения данных о пользователе*/
  const getUser = async () => {
    try {
      const response = await fetch("api/account/isauthenticated");
      const a = await response.json();
      const tempUser = a.user;
      switch (response.status) {
        case 401:
          //navige("/login");
          break;
        case 200:
          setUser(tempUser); //получаем в респонсе помимо user ещё данные. resp {user{}, others}
          break;
        default:
          break;
      }
      console.log("Index.js getUser():", response.error);
    } catch (error) {
      console.error("Error fetching user data:", error.message);
    }
  };

  /*вызов getUser() при каждом обновлении состояния setUser */
  /*при успешной аутентификации пользователя компонент будет 
  вызывать функцию getUser для получения актуальной информации
   о пользователе с сервера.*/
  useEffect(() => {
    getUser();
  }, [setUser]);

  /*Функция добавления занятия в общий список, содержащий
   все предыдущие элементы lessons. Новый элемент lesson 
   будет добавлен в конец массива. */
  const addLesson = (lesson) => setLessons([...lessons, lesson]);

  /*RemoveLesson удаляет урок с указанным removeId из списка уроков lessons */
  const removeLesson = (removeId) =>
    setLessons(lessons.filter(({ id }) => id !== removeId));

  return (
    <ErrorBoundary>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Layout user={user} />}>
            <Route
              index
              element={
                <div>
                  <h3>Главная страница</h3>
                  <Row justify="center">
                    <Col span={12}>
                      <Title level={2}>
                        Добро пожаловать в автошколу "Driving School"!
                      </Title>
                      <Paragraph>
                        Мы предлагаем качественное обучение вождению для всех
                        желающих. Наша автошкола обеспечивает удобное и
                        эффективное обучение как для студентов, так и для
                        преподавателей.
                      </Paragraph>
                      <Paragraph>
                        Наши преимущества:
                        <ul>
                          <li>
                            Профессиональные инструкторы с многолетним опытом.
                          </li>
                          <li>
                            Гибкий график занятий, который подходит для всех.
                          </li>
                          <li>
                            Современные автомобили и оборудование для обучения.
                          </li>
                          <li>Индивидуальный подход к каждому студенту.</li>
                        </ul>
                      </Paragraph>
                    </Col>
                  </Row>
                </div>
              }
            />

            <Route
              path="/lessons"
              element={
                <>
                  <LessonCreate
                    user={user}
                    addLesson={addLesson}
                    teachers={teachers}
                    setTeachers={setTeachers}
                  />
                  <Lesson
                    user={user} //~прокидываем фукнции\поля в формы-компоненты
                    lessons={lessons}
                    teachers={teachers}
                    setLessons={setLessons}
                    removeLesson={removeLesson}
                  />
                </>
              }
            />
            <Route
              path="/login"
              element={
                <LogIn user={user} setUser={setUser} getUser={getUser} />
              }
            />
            <Route
              path="/logoff"
              element={<LogOff user={user} setUser={setUser} />}
            />
            <Route path="*" element={<h3>404</h3>} />
            <Route
              path="/reguser"
              element={
                <RegUser user={user} setUser={setUser} getUser={getUser} />
              }
            />
          </Route>
        </Routes>
      </BrowserRouter>
    </ErrorBoundary>
  );
};
const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  // <React.StrictMode>
  <App />
  // </React.StrictMode>
);
