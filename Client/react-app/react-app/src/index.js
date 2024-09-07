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

const App = () => {
  //функциональный компонент
  const [lessons, setLessons] = useState([]); //lessons Хранит состояние компонента, setLsns - позволяет изменять значение переменной
  const [teachers, setTeachers] = useState([]);
  const [user, setUser] = useState({ isAuthenticated: false, userName: "", userRole: "", id: ""});



  // useEffect(() => {
    const getUser = async () => {
      return await fetch("https://localhost:7230/api/account/isauthenticated")
        .then((response) => {
          response.status === 401 &&
            setUser({ isAuthenticated: false, userName: "" });
          return response.json();
        })
        .then(
          (data) => {
            if (
              typeof data !== "undefined" &&
              typeof data.userName !== "undefined"
            ) {
              setUser({ isAuthenticated: true, userName: data.userName, id: data.id });
            }
          },
          (error) => {
            console.log(error);
          }
        );
    };
    //getUser();
  // }, [setUser]);

  useEffect(() => {
    getUser();
  }, [setUser]);

  const addLesson = (lesson) => setLessons([...lessons, lesson]);
  const removeLesson = (removeId) =>
    setLessons(lessons.filter(({ id }) => id !== removeId));
    
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout user={user} />}>
          <Route index element={<h3>Главная страница</h3>} />
          
          <Route
            path="/lessons"
            element={
              <>
                <LessonCreate
                   user = {user}
                  addLesson={addLesson}
                  teachers={teachers}
                  setTeachers={setTeachers} />
                <Lesson
                  user = {user} //~прокидываем фукнции\поля в формы
                  lessons={lessons}
                  setLessons={setLessons}
                  removeLesson={removeLesson}
                />
              </>
            }
          />
          <Route
            path="/login"
            element={<LogIn user={user} setUser={setUser} getUser={getUser} />}
          />
          <Route path="/logoff" element={<LogOff setUser={setUser} />} />
          <Route path="*" element={<h3>404</h3>} />
          <Route
            path="/reguser"
            element={<RegUser user={user} setUser={setUser} getUser={getUser} />}
          />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};
const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  // <React.StrictMode>
  <App />
  // </React.StrictMode>
);
