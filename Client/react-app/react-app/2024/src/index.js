import React, { useState } from "react";
import ReactDOM from "react-dom/client";

import Blog from "./Components/Blog/Blog";
import CreateBlog from "./Components/CreateBlog/CreateBlog"

// Главный компонет App
const App = () => {
  // Состояние (useState) списка блогов и его изменение
  // Первоначально список блогов пустой
  const [blogs, setBlogs] = useState([]);
  // Функция создания блога на клиенте
  const addBlog = (blog) => setBlogs([...blogs, blog]);
  //Функция удаления блога на клиенте
  const removeBlog = (removeId) =>
    setBlogs(blogs.filter(({ blogId }) => blogId !== removeId));

  return (
    <>
      <CreateBlog addBlog={addBlog} />
      {/* Отобразить компонент блогов и передать в props состояние список блогов 
      и функцию изменения списка блогов */}
      <Blog blogs={blogs} setBlogs={setBlogs} removeBlog={removeBlog} />
    </>
  );
};

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  // <React.StrictMode>
  <App />
  // </React.StrictMode>
);
