// Компонент для отображения списка постов
// Каждый пост содержит postId, title, content
const Post = ({ posts }) => {
  return (
    <>
      {posts.map(({ postId, title, content }) => (
        <div key={postId} id={postId}>
          {title} <br />
          {content} <hr />
        </div>
      ))}
    </>
  );
};

export default Post;
