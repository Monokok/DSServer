import React from "react";
import "./Style.css"; // import "react-widgets/styles.css";
import { useEffect } from "react";
import { Select } from "antd";
import { useState } from "react";
import { Modal } from "antd";

const LessonCreate = ({ user, addLesson, teachers, setTeachers }) => {
  /*Для работы выпадающего списка*/
  const { Option } = Select;

  /*Выбранный день для записи на занятие*/
  const [selectedDateTime, setSelectedDateTime] = useState();

  const [nameOfSelectedTeacher, setNameOfSelectedTeacher] =
    useState("noneName"); //для имени которое выбрали в выпадающем листе (необяз. параметр в фетче)

  /*Для id преподавателя, который выбран в выпадающем листе*/
  const [idOfSelectedTeacher, setIdOfSelectedTeacher] = useState("noneId");

  /*Для списка доступных категорий*/
  const categories = ["A", "B", "C"]; // список категорий
  //Тот же список из Teacher'ов, но только те, которые преподают выбранную категорию обучения
  const [selectedCategoryTeachers, setSelectedCategoryTeachers] = useState([]);

  //выбранная категория в выпадающем списке
  const [selectedCategory, setSelectedCategory] = useState([]);

  /*Для выбранной даты для записи список (где хранятся доступные часы), сеттер и метод для получения с бэка*/
  const [AvailableHours, setAvailableHours] = useState([]);
  /*State для выбранного из списка часа для записи на занятие к преподавателю*/
  const [choosenHour, setChoosenHour] = useState();

  /*Ф-ция получения доступных для записи на выбранную дату ЧАСОВ для занятий */
  const getAvailableHours = async (selectedTeacherId, selectedDate) => {
    try {
      const requestOptions = {
        method: "GET",
      };
      //console.log("DATA В ФЕТЧЕ", selectedDate);
      return await fetch(
        `/api/lessons/${selectedTeacherId}/${selectedDate}`,
        requestOptions
      )
        .then((response) => {
          if (!response.ok) {
            throw new Error(
              "Ошибка загрузки данных метода getAvailableHours()"
            );
          }
          return response.json();
        })
        .then((data) => {
          // Обработка полученных данных
          console.log("AvalaibleHours:", data);
          console.log(`/api/lessons/${selectedTeacherId}/${selectedDate}`);
          setAvailableHours(data); // Изменение состояния списка часов для записи
          if (data.length === 0) {
            Modal.warning({
              title: "Внимание!",
              content: (
                <label>На выбранное число более нет свободных записей!</label>
              ),
            });
            console.log("На выбранное число более нет свободных записей!");
          }
        });
    } catch (error) {
      console.error("Ошибка при загрузке данных:", error);
      // Показать модальное окно с ошибкой
      Modal.error({
        title: "Ошибка",
        content: error.message,
      });
    }
  };

  /*Получение списка преподавателей. Получение данных о преподавателях с сервера
  при первом рендеринге компонента или при setTeachers*/
  useEffect(() => {
    const getTeachers = async () => {
      const requestOptions = {
        method: "GET",
      };
      try {
        // Выполняем запрос к серверу для получения списка преподавателей
        const response = await fetch("api/teachers", requestOptions);

        // Проверяем, успешно ли выполнен запрос
        if (!response.ok) {
          throw new Error("Ошибка при загрузке данных"); // Если запрос не успешен, генерируем ошибку
        }

        // Преобразуем ответ в формат JSON
        const data = await response.json();

        // Выводим полученные данные в консоль для отладки
        console.log("Teachers:", data);

        // Обновляем состояние списка преподавателей с помощью полученных данных
        setTeachers(data);
      } catch (error) {
        // Обрабатываем ошибку, если она произошла во время выполнения запроса или преобразования ответа
        console.error(error);

        // Отображаем модальное окно с сообщением об ошибке
        Modal.error({
          title: "Ошибка",
          content:
            "Произошла ошибка при загрузке данных о преподавателях. Пожалуйста, попробуйте позже.",
        });
      }
    };

    // Вызываем функцию получения списка преподавателей
    getTeachers();
  }, [setTeachers]); // зависимость - функцию для обновления состояния списка преподавателей

  /*обработка нажатия кнопки "создать запись на занятие"*/
  const handleSubmit = (e) => {
    e.preventDefault();
    console.log("Lesson create: id Teacher'a: ", idOfSelectedTeacher);

    if (idOfSelectedTeacher === "noneId" || idOfSelectedTeacher === undefined) {
      Modal.error({
        title: "Ошибка",
        content: "Не выбран преподаватель!",
      });
      return;
    }

    var value = new Date(e.target.elements.selectedDate.value); //достали дату из селекта
    if (value && isNaN(value.getTime())) {
      // Если выбранная дата некорректна
      Modal.error({
        title: "Ошибка",
        content: "Не выбран день занятия!",
      });
      return;
    }
    console.log("selectedDate.value", value);

    console.log("value", value);

    let month = parseInt(value.getMonth()); // Месяцы в объекте Date начинаются с 0
    console.log("month", month);

    let day = parseInt(value.getDate()); //НЕ getDay()!!!!!!
    console.log("day", day);

    let year = parseInt(value.getFullYear());
    console.log("year", year);
    console.log("ChoosenHour", choosenHour);

    if (choosenHour === undefined) {
      Modal.error({
        title: "Ошибка",
        content: "Не выбрано время занятия!",
      });
      return;
    } else {
      //когда все поля корректны:
      // Разбиваем строку времени занятия на компоненты
      let timeComponents = choosenHour.split(":");
      let hours = parseInt(timeComponents[0]);
      let minutes = parseInt(timeComponents[1]);

      let combinedDate = new Date(year, month, day, hours + 3, minutes); //hours + 3 - Т.к. не МСК формат времени создаётся.
      const lesson = {
        date: combinedDate,
        student_id: user.id, //"519512e2-3129-4987-9633-c311d8a0f47e",
        teacher_id: idOfSelectedTeacher, //"ebacf089-5709-47c1-8c29-8f1e62ec72ca",
        type_id: 1,
        cathegory_id: selectedCategory,
        teacherName: nameOfSelectedTeacher,
      };

      console.log(JSON.stringify(lesson));

      const createLesson = async () => {
        try {
          const requestOptions = {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(lesson), //value
          };
          const response = await fetch("api/lessons", requestOptions);

          console.log("\n", JSON.stringify(requestOptions));
          return await response.json().then((data) => {
            console.log(data);
            // response.status === 201 && addLesson(data)
            if (response.ok) {
              addLesson(data);
              //после успешной записи на занятие, необходимо обновить список
              getAvailableHours(idOfSelectedTeacher, selectedDateTime);
              //обновить имя Преподавателя
              setNameOfSelectedTeacher(data.teacherName);
              switch (response.status) {
                case 201:
                  Modal.success({
                    title: "Запись осуществлена",
                    content: "Запись на занятие успешно создана!",
                  });
                  break;

                default:
                  break;
              }
            } else {
              console.log("Responce status is not OK CreateLesson()", response.status);
              //в случае, если ответ от сервера не успешный, выбрасываем ошибку
              switch (response.status) {
                case 400:
                  Modal.error({
                    title: "Ошибка записи на занятие",
                    content:
                      "Возникла ошибка при отправке запроса. Пожалуйста, убедитесь, что вы правильно заполнили все поля и повторите попытку.",
                  });
                  break;
                case 409:
                  Modal.error({
                    title: "Ошибка записи на занятие",
                    content:
                      "Извините, выбранное вами время уже занято.\nПожалуйста, выберите другое доступное время для записи!",
                  });
                  break;
                case 422:
                  Modal.error({
                    title: "Ошибка записи на занятие",
                    content:
                      "Выбранный преподаватель не предоставляет обучение по выбранной категории. Пожалуйста, выберите другого преподавателя или свяжитесь с администратором автошколы для получения дополнительной информации.",
                  });
                  break;
                case 406:
                  Modal.error({
                    title: "Ошибка записи на занятие",
                    content:
                      "Выбранная дата неактуальна. Проверьте правильность заполнения данных!",
                  });
                  break;
                default:
                  break;
              }
              throw new Error("Ошибка при создании занятия");
            }
          });
        } catch (error) {
          console.error(error);
        }
      };
      createLesson();
    }
  };

  //функция обработки события выбора категории в выпадающем списке
  function CathegorySelectHandleChange(event) {
    //сюда поступают выбранные категории. event = A, B, C
    //выбрав категорию - нужно обновить список преподавателей - вывести кто ведёт именно эту категорию
    setSelectedCategoryTeachers(
      //фильтруем и обновляем список преподавателей в зависимости от выбранной категории
      teachers.filter((teacher) => {
        if (event === "A") {
          setSelectedCategory(0);
          return teacher.teachesCategoryA;
        }
        if (event === "B") {
          setSelectedCategory(1);
          return teacher.teachesCategoryB;
        }
        if (event === "C") {
          setSelectedCategory(2);
          return teacher.teachesCategoryC;
        }
        return false; // Возвращаем false, если выбрана пустая категория или др значение
      })
    );
    console.log("Выбрана категория:", event);
  }

  // Функция, которая будет вызываться при изменении значения в <input> с датой
  function handleChange(event) {
    // Получаем значение даты из <input>
    const selectedDate = event.target.value;
    setSelectedDateTime(selectedDate);
    if (idOfSelectedTeacher !== undefined) {
      // Вызываем функцию getAvailableHours() и передаем в неё выбранную дату
      getAvailableHours(idOfSelectedTeacher, selectedDate);
      console.log("Выбранная дата занятия:", selectedDate);
    } else
      console.log(
        "Выберите преподавателя для получения списка доступных часов!"
      );
  }
  /*Установка состояния для выбранного из списка часа занятия */
  function SelectHandleChange(event) {
    //event - дата. н-р: = 2024-05-05T10:00:00
    console.log("Выбранное время:", event);
    //const SelectSelectedHour = event.target.value;

    if (event !== undefined) setChoosenHour(event);
    else console.log("Выбранный час undefined:", event);
  }

  /*Для пользователя системы (Студента автошколы) отображаем компонент:*/
  if (user.isAuthenticated && user.userRole === "user")
    return (
      <React.Fragment>
        {/* {console.log("User Id = ", user.id)} */}
        <h3>Создание новой записи на занятие</h3>
        Выберите категорию<br></br>
        <form onSubmit={handleSubmit}>
          {/* Выпадающий список категории обучения (А, В, С) */}
          <Select
            className="SelectForCathegory"
            placeholder="Выберите категорию обучения"
            // defaultValue="Выберите доступное время"
            style={{
              width: 120 * 3,
            }}
            onChange={(e) => CathegorySelectHandleChange(e)} //при выборе категории - изменяем список тичеров
          >
            {categories.map((category) => (
              <Option key={category} value={category}>
                {category}
              </Option>
            ))}
          </Select>
          <br></br>
          Выберите преподавателя для записи на занятие<br></br>
          <Select
            options={selectedCategoryTeachers.map((teacher) => ({
              value: teacher.id,
              label: `${teacher.last_name} ${teacher.first_name} ${teacher.middle_name}`,
            }))}
            placeholder="Выберите преподавателя"
            style={{ width: 120 * 3 }}
            onChange={(value) => {
              setIdOfSelectedTeacher(value);
              console.log("Выбранный преподаватель имеет Id:", value);
            }}
          ></Select>
          <label>
            <br></br>Выберите дату{" "}
          </label>
          <br></br>
          <input
            type="date"
            style={{ width: "350px" }}
            name="selectedDate"
            placeholder="Введите Дату:"
            onChange={(e) => handleChange(e)}
          />
          <label>
            <br></br>Выберите доступное время
          </label>
          <br></br>
          <div className="container">
            <Select
              allowClear={true}
              className="SelectForChooseHour"
              placeholder="Выберите доступное время"
              // defaultValue="Выберите доступное время"
              style={{
                width: 120 * 3,
              }}
              onChange={(e) => SelectHandleChange(e)}
            >
              {AvailableHours === undefined || AvailableHours === null ? (
                <Option>{undefined}</Option>
              ) : (
                AvailableHours.map((hour) => (
                  <Option key={`${hour}`} value={hour}>
                    {hour}
                  </Option>
                ))
              )}
            </Select>
            <br></br>

            <button className="button" type="submit">
              Создать
            </button>
          </div>
        </form>
        
      </React.Fragment>
    );
  //Для преподавателя отображаем след. пустой компонент:
  else if (user.isAuthenticated && user.userRole === "teacher") {
  } else //если неавторизован показываем список преподавателей
    return (
      <div>
        <h2>Преподаватели автошколы:</h2>
        {teachers.map(
          //map - вывод из массива
          (
            { id, first_name, last_name } //{} для
          ) => (
            <div className="Teacher" key={id} id={id}>
              <li>
                <strong>
                  {/* {" "} */}
                  {/* {id}:  */}
                  {first_name} {last_name}
                </strong>
              </li>
            </div>
          )
        )}
      </div>
    );
};
export default LessonCreate;
