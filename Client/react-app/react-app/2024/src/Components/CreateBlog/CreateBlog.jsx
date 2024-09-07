import React from "react";

const url = "https://localhost:7074/api/blogs";

// Компонент создания нового блога
const BlogCreate = ({ addBlog }) => {
  const handleSubmit = (e) => {
    e.preventDefault();
    const { value } = e.target.elements.url;
    // Создание объекта, который отправится на сервер
    const blog = { url: value };

    const createBlog = async () => {
      // Опции запроса на сервер. В body хранится объект для создания
      const requestOptions = {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(blog),
      };

      // Вызов метода API
      const response = await fetch(url, requestOptions);
      // Обработка ответа от API
      return await response.json().then(
        (data) => {
          console.log(data);
          // response.status === 201 && addBlog(data)
          // Если ответ 200 и создание прошло успешно
          if (response.ok) {
            // Создаем блог на клиенте, чтобы он отобразился
            addBlog(data);
            // Очищение строки создания блога
            e.target.elements.url.value = "";
          }
        },
        (error) => console.log(error)
      );
    };
    createBlog();
  };

  return (
    <>
      <h3>Создание нового блога</h3>
      <form onSubmit={handleSubmit}>
        <label>URL: </label>
        <input type="text" name="url" placeholder="Введите Url:" />
        <button type="submit">Создать</button>
      </form>
    </>
  );
};

export default BlogCreate;
