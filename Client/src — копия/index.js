import React, { useState } from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import LessonCreate from "./components/lessonCreate/lessonCreate";
import Lesson from "./components/lesson/lesson";
import Layout from "./components/layout/layout";
import LogIn from "./components/LogIn/LogIn";
import LogOff from "./components/LogOff/LogOff";
import { useEffect } from "react";

const App = () => {
  //функциональный компонент
  const [lessons, setLessons] = useState([]);
  const [teachers, setTeachers] = useState([]);
  const [user, setUser] = useState({ isAuthenticated: false, userName: "" });

  useEffect(() => {
    const getUser = async () => {
      return await fetch("api/account/isauthenticated")
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
              setUser({ isAuthenticated: true, userName: data.userName });
            }
          },
          (error) => {
            console.log(error);
          }
        );
    };
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
                  addLesson={addLesson}
                  teachers={teachers}
                  setTeachers={setTeachers} />
                <Lesson
                  lessons={lessons}
                  setLessons={setLessons}
                  removeLesson={removeLesson}
                />
              </>
            }
          />
          <Route
            path="/login"
            element={<LogIn user={user} setUser={setUser} />}
          />
          <Route path="/logoff" element={<LogOff setUser={setUser} />} />
          <Route path="*" element={<h3>404</h3>} />
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

//import React from 'react';
// import ReactDOM from 'react-dom/client';
// import './index.css';
// import App from './App';
// import reportWebVitals from './reportWebVitals';

// const root = ReactDOM.createRoot(document.getElementById('root'));
// root.render(
//   <React.StrictMode>
//     <App />
//   </React.StrictMode>
// );

// // If you want to start measuring performance in your app, pass a function
// // to log results (for example: reportWebVitals(console.log))
// // or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
// reportWebVitals();
