import React, { useEffect } from "react";
import Post from "../Post/Post";

// Путь к API серверной части
const url = "https://localhost:7074/api/blogs";

// Компонент блогов принимает список блогов и функцию изменения состояния
const Blog = ({ blogs, setBlogs, removeBlog }) => {
  // Срабатывает перед рендерингом компонента при изменении setBlogs
  useEffect(() => {
    // Выполняется функция получения данных от Сервера
    getBlogs();
  }, [setBlogs]);

  // Получить список блогов
  const getBlogs = () => {
    const requestOptions = {
      method: "GET",
    };
    // Запрос к API
    return fetch(url, requestOptions)
      .then((response) => response.json())
      .then(
        (data) => {
          // data - данные от сервера
          console.log("Data:", data);
          // Изменение состояния списка блогов
          setBlogs(data);
        },
        (error) => {
          console.log(error);
        }
      );
  };

  // Запрос к API для удаления блога
  const deleteBlog = async ({ blogId }) => {
    const requestOptions = {
      method: "DELETE",
    };
    return await fetch(`${url}/${blogId}`, requestOptions).then(
      (response) => {
        // В случае успеха, удаляется блог на клиенте
        if (response.ok) {
          removeBlog(blogId);
        }
      },
      (error) => console.log(error)
    );
  };

  // Вывод списка блогов, каждый из которых содержит blogId, url, post
  function Blogs() {
    return blogs.map(({ blogId, url, post }) => (
      <div className="Blog" key={blogId} id={blogId}>
        <strong>
          {blogId}: {url}
        </strong>

        {/* Кнопка удаления блога, которая вызывает функцию удаления с id */}
        <button onClick={() => deleteBlog({ blogId })}>Удалить</button>

        {/* Вывод списка постов для блога, если они есть */}
        {post && <Post posts={post} />}
      </div>
    ));
  }
  return (
    <>
      <h3>Список блогов</h3>
      <Blogs />
    </>
  );
};

export default Blog;
