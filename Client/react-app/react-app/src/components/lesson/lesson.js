import "./Style.css";
import React, { useEffect } from "react";
import "./Style.css";

// Путь к API серверной части
//const url = "https://localhost:7230/api/";

// Компонент занятий принимает список занятий и функцию изменения состояния
const Lesson = ({ user, setUser, lessons, setLessons, removeLesson }) => {
  //состояние и два метода: деструктурирующее присваивание JavaScript и присвоение значений свойств локальным переменным
  //Функция, переданная в useEffect, будет запущена после того, как рендер будет зафиксирован на экране
  // Срабатывает перед рендерингом компонента при изменении setLessons
  useEffect(() => {
    console.log("Эффектовская хуйня user.id:", user.id);
    // Выполняется функция получения данных от Сервера
    const getLessons = async () => {
      //функция обратного вызова, которая вызывается React в подходящее время;
      const requestOptions = {
        method: "GET",
      };
      return await fetch(
        `https://localhost:7230/api/lessons/${user.id}`,
        requestOptions
      )
        .then((response) => response.json())
        .then(
          (data) => {
            // data - данные от сервера
            console.log("Lessons:", data);
            //console.log("setLessons:", setLessons);

            setLessons(data); // Изменение состояния списка занятий
          },
          (error) => {
            console.log(error);
          }
        );
    };
    getLessons();
  }, [setLessons]); //массив зависимостей (необязательный)
  //Список зависимостей содержит переменные, которые будут проверяться перед обратным вызовом и определять, следует ли вообще его делать.

  const deleteItem = async ({ id }) => {
    //объявление функции (асинхронной) в которую передаём id
    const requestOptions = { method: "DELETE" };
    return await fetch(
      `https://localhost:7230/api/Lessons/${id}`,
      requestOptions
    ).then(
      //фнуукция вернёт промисс (из-за async)
      (response) => {
        //response - ответ от сервера?
        // выполнится, когда от API придет ответ
        if (response.ok) {
          //структура вернувшегося и его поле ok
          removeLesson(id);
        }
      },
      (error) => console.log(error) //error - аналогично
    );
  };

  if (user.isAuthenticated)
    return (
      // <React.Fragment>
      //   <h3>Список занятий</h3>
      //   {lessons.map((lesson) => (
      //     <div className="Lesson" key={lesson.id} id={lesson.id}>
      //       <strong>
      //         {" "}
      //         {lesson.id}: {lesson.date}{" "}
      //       </strong>
      //       <button onClick={(e) => deleteItem(lesson.id)}>Удалить</button>
      //       {lesson.student.map((student) => (
      //         <div className="student" key={student.id} id={student.id}>
      //           {student.middleName} <br />
      //           {student.firstName} <hr />
      //           {student.lastName} <hr />
      //         </div>
      //       ))}
      //     </div>
      //   ))}
      // </React.Fragment>
      <React.Fragment>
        <h3>Список всех занятий:</h3>
        {lessons.map(
          //map - вывод из массива
          (
            { id, date, student } //{} для
          ) => (
            <div className="Lesson" key={id} id={id}>
              <li>
                <strong>
                  {" "}
                  {id}: {date}{" "}
                </strong>
                {console.log("User Auth = ", user.isAuthenticated)}
                {user.isAuthenticated ? (
                  <button
                    onClick={(e) => deleteItem({ id }).then(console.log(id))}
                  >
                    Удалить
                  </button>
                ) : (
                  ""
                )}
              </li>

              {/* {student.map(({id, firstName, lastName, middleName }) => (
              <div className="student" key={id} id={id}>
                {middleName} <br />
                {firstName} <hr />
                {lastName} <hr />
              </div>
            ))} */}
            </div>
          )
        )}
      </React.Fragment>
    );
  else return <h4>Чтобы узнать о предстоящих занятиях, войдите в аккаунт!</h4>;
};
export default Lesson;
